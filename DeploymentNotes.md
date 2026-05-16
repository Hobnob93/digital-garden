# Digital Garden — Deployment Notes

Reference guide for how this application is deployed and operated. Covers the CI/CD pipeline, what each container does, common issues hit during setup with their resolutions, and a cheat sheet of useful server commands.

---

## How deployment works

The application is deployed via **GitHub Actions** to a single Hetzner Cloud VPS, defined entirely by `docker-compose.yml` at the repo root.

### Pipeline overview (`.github/workflows/ci-deploy.yml`)

The workflow has three sequential jobs:

1. **`test`** — Runs on every push and PR. Restores, builds, and runs the xUnit test suite. Publishes results as a GitHub check.
2. **`build`** — Runs only on push to `main` (skipped for PRs). Builds the Docker image from the repo's `Dockerfile` and pushes it to GitHub Container Registry (`ghcr.io/<owner>/<repo>`) with both `latest` and a SHA-based tag.
3. **`deploy`** — SSHes into the Hetzner server, copies `docker-compose.yml` and a freshly generated `.env`, then runs `docker compose pull && docker compose up -d`.

### The `.env` file

The deploy job constructs `.env` on the GitHub runner from repository secrets, then `scp`s it to the server. The file is never committed to source control.

**Bcrypt hashes need `$` doubled.** Any secret containing `$` characters (the `*_BASIC_AUTH` secrets for Traefik's basic-auth middleware) is escaped via `sed 's/\$/$$/g'` so docker-compose's variable substitution leaves a single `$` for Traefik. Store the **raw `htpasswd` output** in GitHub Secrets — the escaping happens automatically in CI.

### Concurrency & scheduling

- `concurrency: prod-deploy` with `cancel-in-progress: true` — two pushes in quick succession won't race; the newer one wins.
- A weekly cron at `0 3 * * 1` (Mondays 03:00 UTC) triggers a rebuild to pick up base-image security patches.
- `workflow_dispatch` allows manual triggering from the Actions tab.

---

## Services

All services run on a shared Docker network (`root_default`) and are fronted by Traefik. Only Traefik exposes ports to the host (80 and 443).

| Service | Purpose |
|---|---|
| **web** | The Blazor .NET application itself. Connects to `postgres` for data. |
| **traefik** | Reverse proxy. Routes incoming HTTPS traffic to the correct service based on hostname labels, handles Let's Encrypt cert issuance/renewal, redirects HTTP→HTTPS. |
| **postgres** | PostgreSQL 16. Backs both the web app and n8n (separate databases). Not exposed publicly — only reachable from inside the docker network. |
| **pgadmin** | Web UI for managing Postgres at `db.<domain>`. Behind Traefik basic auth + optional IP whitelist. |
| **seq** | Structured log aggregation at `logs.<domain>`. The web app ships logs to it via Serilog. Behind Traefik basic auth + optional IP whitelist. |
| **media** | An nginx serving `/srv/media` read-only at `media.<domain>`. Public — for media files referenced from the site. |
| **filebrowser** | Web UI for uploading/managing files in `/srv/media` at `files.<domain>`. Behind Traefik basic auth + optional IP whitelist, with its own internal user system on top. |
| **n8n** | Workflow automation at `n8n.<domain>`. Receives webhooks from external services, so deliberately not behind basic auth or IP whitelist — relies on its own login. |

### Volumes

| Volume | Contents | Critical? |
|---|---|---|
| `pg-data` | PostgreSQL data files (all databases) | **Yes** — losing this loses all app data and n8n workflows |
| `traefik-acme` | Let's Encrypt certificates and ACME account key | Important — losing it triggers re-issuance, which has [rate limits](https://letsencrypt.org/docs/rate-limits/) |
| `n8n-data` | n8n's local config (`.n8n` directory) | Less critical — main n8n state is in Postgres, but encryption key fingerprint is here |
| `filebrowser-db` | Filebrowser's user accounts/settings | Recoverable (admin password regenerates) but loses user accounts |
| `seq-data` | Seq's log storage | Disposable — logs only |
| `file-logs` | App's own log files | Disposable — also shipped to Seq |

`/srv/media` is a **bind mount**, not a Docker volume — it lives directly at `/srv/media` on the host. Back it up with rsync, not Docker volume tooling.

### Networks

A single network, `root_default`, declared in the compose file. All services join it; service-to-service communication uses container names as hostnames (e.g. the web app connects to Postgres at `Host=postgres`).

---

## Common issues and resolutions

### Basic auth secrets — the `$$` escape

**Symptom:** Basic auth login fails despite the password being correct, or container fails to start with parsing errors.

**Cause:** The `*_BASIC_AUTH` secrets contain bcrypt hashes with literal `$` characters. Compose treats `$VAR` as variable substitution, so a hash like `$2y$05$...` gets corrupted.

**Solution:** Store the raw `htpasswd -nbB user 'password'` output in GitHub Secrets. The deploy workflow doubles `$` to `$$` automatically when writing `.env`. Don't pre-escape.

**Verify what Traefik actually sees:**
```bash
docker inspect $(docker compose ps -q seq) | grep "basicauth.users"
```
Should show single `$` characters, not `$$`.

### Browser caching basic auth credentials

**Symptom:** Updated basic auth password, but login still fails — even with correct credentials.

**Cause:** Browsers aggressively cache basic auth credentials per-origin and silently re-submit them. The cache survives even when you're being prompted again.

**Solution:** Always test basic-auth changes in an **incognito/private window**. To purge in normal Chrome: `chrome://settings/clearBrowserData` → cookies and cached files for the affected time range.

### Filebrowser password lost on first run

**Symptom:** Can't log into Filebrowser's own UI; `admin`/`admin` doesn't work.

**Cause:** Recent Filebrowser versions generate a random admin password on first boot, printed to logs **once**.

**Solution if logs still show it:**
```bash
docker compose logs filebrowser | grep -i password
```

**Recovery if logs have rotated:**
```bash
docker compose rm -f filebrowser
docker volume rm digital-garden_filebrowser-db
docker compose up -d filebrowser
docker compose logs filebrowser | grep -i password
```
**Save the new password to 1Password immediately**, then change it via Filebrowser's UI to something memorable.

### `/srv/media` permissions — 403 on Filebrowser uploads

**Symptom:** Filebrowser shows 403 when uploading files.

**Cause:** Filebrowser runs as UID 1000 inside the container; `/srv/media` is owned by `root:root` on the host.

**Solution:**
```bash
chown -R 1000:1000 /srv/media
```

### `/srv/media` permissions — 403 from nginx

**Symptom:** Files visible in Filebrowser but `media.<domain>/<file>` returns 403.

**Cause:** nginx workers run as UID 101; files are only readable by their owner (1000).

**Solution:**
```bash
chmod -R a+rX /srv/media
```
Capital `X` adds execute (= directory-traverse) permission only on directories and already-executable files. Future Filebrowser uploads default to `644` and will be readable by nginx automatically.

### pgAdmin shows no servers

**Symptom:** pgAdmin loads but the server list is empty.

**Cause:** pgAdmin doesn't auto-discover Postgres — connections must be registered manually once per pgAdmin instance.

**Solution:** Right-click **Servers → Register → Server**. Use:
- **Host:** `postgres` (the docker service name, *not* localhost)
- **Port:** `5432`
- **Maintenance database / Username / Password:** from `PG_DB` / `PG_USER` / `PG_PASSWORD` in `.env`

### DNS records and Let's Encrypt

**Symptom:** New subdomain shows Traefik's default self-signed cert and won't issue a real one.

**Cause:** Let's Encrypt verifies domain ownership via DNS — the record must exist and resolve to this server before ACME can succeed.

**Solution:** Add an A record at the DNS provider pointing the subdomain at the Hetzner server IP. Within 5–30 minutes Traefik will retry the ACME challenge and obtain a real cert. Check with:
```bash
docker compose logs traefik | grep -i acme
```

### n8n encryption key

**Not a symptom — a warning.** Losing `N8N_ENCRYPTION_KEY` makes every stored credential in n8n permanently unreadable. It encrypts API keys, OAuth tokens, and passwords used by workflows. Store it in 1Password. Never regenerate it on a running instance.

---

## Server command cheat sheet

All commands assume you're SSHed into the Hetzner server. Most assume you're in `/srv/digital-garden`.

### SSH
```bash
ssh root@<server-ip>
```
1Password's SSH agent supplies the key; no `-i` flag needed once the agent is configured.

### Container status & logs
```bash
docker compose ps                          # All containers and their state
docker compose logs <service>              # Recent logs from one service
docker compose logs -f <service>           # Follow logs live
docker compose logs <service> | tail -100  # Last 100 lines
docker compose logs --since 10m <service>  # Last 10 minutes only
```

### Restarting & recreating
```bash
docker compose restart <service>                  # Quick restart, same container
docker compose up -d <service>                    # Apply config changes
docker compose up -d --force-recreate <service>   # Force fresh container (use when env vars changed but not picked up)
docker compose pull <service> && docker compose up -d <service>   # Pull new image and apply
```

### Inspect what Traefik sees
```bash
# View the actual labels on a running container (after compose env substitution)
docker inspect $(docker compose ps -q <service>) | grep -A 20 Labels

# View a specific label
docker inspect $(docker compose ps -q seq) | grep "basicauth.users"
```

### Network checks
```bash
docker network ls                                  # All networks
docker network inspect root_default                # Containers attached to the network
docker compose exec <service> getent hosts <other-service>   # DNS check between containers
```

### Disk usage
```bash
df -h                          # Host disk usage
docker system df               # Docker-specific space usage
docker system df -v            # Detailed: per-volume sizes
du -sh /srv/media              # Size of the media bind mount
du -sh /var/lib/docker         # Total Docker storage
```

### Postgres operations
```bash
# Open psql session
docker compose exec postgres psql -U "$PG_USER" -d "$PG_DB"

# List all databases
docker compose exec postgres psql -U "$PG_USER" -l

# Run a one-off query
docker compose exec postgres psql -U "$PG_USER" -d "$PG_DB" -c "SELECT count(*) FROM some_table;"

# Backup
docker compose exec -T postgres pg_dump -U "$PG_USER" -d "$PG_DB" -Fc > backup-$(date +%Y%m%d).dump

# Restore (drops + recreates objects)
cat backup.dump | docker compose exec -T postgres pg_restore -U "$PG_USER" -d "$PG_DB" --clean --if-exists
```

### File permissions reset (when needed)
```bash
chown -R 1000:1000 /srv/media
chmod -R a+rX /srv/media
```

### Generate new basic-auth credentials
```bash
htpasswd -nbB <username> '<password>'   # Single quotes around password!
```
Paste the entire output line (`username:$2y$05$...`) into the relevant GitHub Secret.

### Cleanup
```bash
docker compose pull                # Pull latest of everything
docker image prune -af             # Remove dangling images
docker volume ls                   # List all volumes (be careful before removing)
docker system prune -af            # Aggressive cleanup — review what it'll delete first
```

### Firewall
```bash
ufw status                  # Current rules
ufw allow <port>/tcp        # Open a port
ufw delete allow <port>/tcp # Close a port
```

### System updates
```bash
apt update && apt upgrade -y
reboot                      # If kernel updates were applied
```

---

## Recovery scenarios

**App is down, unsure why:**
1. `docker compose ps` — is anything not Running?
2. `docker compose logs --tail 50 <service>` for the failing service
3. `docker compose logs --tail 50 traefik` for routing issues

**Need to roll back to a previous image:**
The `build` job tags with both `latest` and the commit SHA. To pin to an older version, temporarily change the image tag in `docker-compose.yml` to `ghcr.io/<owner>/<repo>:sha-<short-sha>` and `docker compose up -d web`.

**Full server replacement:**
1. Provision new server, install Docker, set up firewall and SSH access
2. Update `SSH_HOST` GitHub Secret to new IP
3. Run pipeline (it provisions an empty stack)
4. Restore Postgres dump and rsync `/srv/media` from backup
5. Update DNS A records to point at new server
6. Reapply `chown` and `chmod` on `/srv/media`