

function randomlySizeHeroTiles() {
    for (let i = 1; i <= 4; i++) {
        var top = document.querySelector('.hero-tile.first-word.letter-' + i);
        var bottom = document.querySelector('.hero-tile.second-word.letter-' + i);

        var topHeight = randomIntFromInterval(41, 59);
        var bottomHeight = 100 - topHeight;

        top.style.setProperty('--hero-tile-height', topHeight + 'vh');
        bottom.style.setProperty('--hero-tile-height', bottomHeight + 'vh');
    }
}

function randomIntFromInterval(min, max) { // min and max included 
    return Math.floor(Math.random() * (max - min + 1) + min)
}

function initialiseHeroTileEvents() {
    let hoverMediaQuery = window.matchMedia("(hover: hover)");

    if (hoverMediaQuery.matches) {
        setupHoverEvents();
    } else {
        setupTouchEvents();
    }
}

function setupTouchEvents() {
    $('.hero-tile')
        .unbind()
        .click(function () {
            let target = $(this);

            $('.hero-tile').each(function () {
                let loopTarget = $(this);
                if (loopTarget.is(target) === false) {
                    if (loopTarget.hasClass('active')) {
                        loopTarget.trigger('click');
                    }
                }
            });

            if (target.hasClass('active')) {
                target.removeClass('active');

                onTileUnfocus();
            } else {
                target.addClass('active');

                onTileFocus(target);
            }
        });
}

function setupHoverEvents() {
    $('.hero-tile')
        .unbind()
        .mouseenter(function (e) {
            let target = $(this);
            onTileFocus(target);
        })
        .mouseleave(function (e) {
            onTileUnfocus();
        });
}

function onTileFocus(target) {
    let screenSizeMediaQuery = window.matchMedia("only screen and (max-width: 600px)");

    if (screenSizeMediaQuery.matches) {
        $('.hero-tile').each(function () {
            let loopTarget = $(this);

            if (loopTarget.is(target) === false) {
                loopTarget.addClass('hide');
            }
        });
    } else {
        let topBottomClass = (target.hasClass('first-word') ? 'first-word' : 'second-word');

        $('.hero-tile.' + topBottomClass).each(function () {
            let loopTarget = $(this);

            if (loopTarget.is(target) === false) {
                loopTarget.addClass('hide');
            }
        });
    }
}

function onTileUnfocus() {
    $('.hero-tile').each(function () {
        $(this).removeClass('hide');
    });
}

function mediaQueryChanged() {
    $('.hero-tile').each(function () {
        let loopTarget = $(this);
        if (loopTarget.hasClass('active')) {
            loopTarget.trigger('click');
        }
    });

    initialiseHeroTileEvents();
}

function setupHeroTiles() {
    randomlySizeHeroTiles();
    initialiseHeroTileEvents();

    var hoverQuery = window.matchMedia("(hover: hover)");
    var sizeQuery = window.matchMedia("only screen and (max-width: 600px)");
    hoverQuery.addEventListener('change', () => mediaQueryChanged());
    sizeQuery.addEventListener('change', () => mediaQueryChanged());
}