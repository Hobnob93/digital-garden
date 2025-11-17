using DigitalGarden.Shared.Models.Data;

namespace DigitalGarden.Client.Components.Pages;

public partial class GiftList
{
    private BeaconCategoryItems[] CategoryItems => [
        new BeaconCategoryItems
        {
            Name = "Gadgets",
            Beacons = [
                new Beacon
                {
                    Title = "Any Wallet from Secrid",
                    Description = "Only preference is that it's black; other than that, I like them all!",
                    Url = "https://secrid.com/en-gb"
                },
                new Beacon
                {
                    Title = "Nuovva Knife Set",
                    Description = "Needs to be stainless steel.",
                    Url = "https://www.amazon.co.uk/Kitchen-Knife-Set-Rotating-Block/dp/B08YYXZ1TJ?tag=theevenstan-21&ascsubtag=ES%7C1051035%7CB08YYXZ1TJ&geniuslink=true&th=1"
                },
                new Beacon
                {
                    Title = "Developer Evolution Mug",
                    Description = "Preferably the one with black internals.",
                    Url = "https://www.amazon.co.uk/CafePress-Computer-Programmer-Ceramic-Coffee/dp/B07SSCKWFS/ref=sr_1_57?crid=28HCVP33MYZZ4&dib=eyJ2IjoiMSJ9.YU7At2HNM3cqv8p_aOA56pmGec7dsrMqBP3p848_MyjPnqSxdZWXs7hgw_1n769xzPWueGnzmp5EZ6GWn2gDMcjQIuNBsdr1TakMePU0oLVHZeHCH6Bc33a8BzhwTxbQAXcyG2HWeKBIJhoZ3UdFXlHM-KM63Q6SOulF89zmomk21gfZGxut6i-kxw2G7Rd4CxllGburREVxtSFS92WqGNV1neCqOK4hd1M6YdTdYxEdu5xaM6qhyv8ksPvLVTIElarNTOTgm9bLWF4_v4TUJRWw_BX4e0OaoOlR4b7Odso.HgSXnnMNsZ0fSr2BlLTSMC6VTOxQ0TqyuvCMcK-HMdU&dib_tag=se&keywords=programmer%2Blarge%2Bmug&qid=1762376768&sprefix=programmer%2Blarge%2Bmug%2Caps%2C88&sr=8-57&th=1"
                },
                new Beacon
                {
                    Title = "Mechanical Tellurian Wooden Puzzle",
                    Description = "This is a Ukrainian company, so delivery times may reflect this.",
                    Url = "https://ugearsmodels.com/products/mechanical-tellurion?_pos=9&_fid=8b6af3dc3&_ss=c"
                },
                new Beacon
                {
                    Title = "NASA Space Shuttle Wooden Puzzle",
                    Description = "This is a Ukrainian company, so delivery times may reflect this.",
                    Url = "https://ugearsmodels.com/collections/gifts-for-him/products/nasa-space-shuttle-discovery"
                }
            ]
        },
        new BeaconCategoryItems
        {
            Name = "Clothes",
            Beacons = [
                new Beacon
                {
                    Title = "Medium Hoodie (Navy Blue)",
                    Description = "Medium and 'Loose Fit' because hoodies are uncomfortable when they shrink!",
                    Url = "https://www2.hm.com/en_gb/productpage.0970819117.html"
                },
                new Beacon
                {
                    Title = "Medium Hoodie (Light Grey)",
                    Description = "Medium and 'Loose Fit' because hoodies are uncomfortable when they shrink!",
                    Url = "https://www2.hm.com/en_gb/productpage.0970819007.html"
                },
                new Beacon
                {
                    Title = "Medium Hoodie (Plum)",
                    Description = "Medium and 'Loose Fit' because hoodies are uncomfortable when they shrink!",
                    Url = "https://www2.hm.com/en_gb/productpage.0970819114.html"
                },
                new Beacon
                {
                    Title = "Medium 7-pack T-Shirts",
                    Description = "In size medium as they are susceptible to slight shrinkage...",
                    Url = "https://www.marksandspencer.com/7pk-pure-cotton-crew-neck-t-shirts/p/clp60591761?color=WHITEMIX#intid=pid_pg1pip48g4r2c3%7Cprodflag_plp_ts_QP_100+"
                }
            ]
        },
        new BeaconCategoryItems
        {
            Name = "Edibles",
            Beacons = [
                new Beacon
                {
                    Title = "Cheddar Rum: The Honey One",
                    Description = "Link is to their general page as can't seem to link to the exact product...",
                    Url = "https://thecheddarspiritco.co.uk/cheddar-rum/"
                },
                new Beacon
                {
                    Title = "Cheddar Rum: Coffee Caramel",
                    Description = "Link is to their general page as can't seem to link to the exact product...",
                    Url = "https://thecheddarspiritco.co.uk/cheddar-rum/"
                },
                new Beacon
                {
                    Title = "Cheddar Rum: Banana",
                    Description = "Link is to their general page as can't seem to link to the exact product...",
                    Url = "https://thecheddarspiritco.co.uk/cheddar-rum/"
                },
                new Beacon
                {
                    Title = "Salted Caramel Hot Chocolate",
                    Description = "Doesn't have to be M&S ones; I'm open to other brands!",
                    Url = "https://www.ocado.com/products/m-collection-salted-caramel-flavour-drinking-chocolate/673687011"
                },
                new Beacon
                {
                    Title = "Belgian Hot Chocolate",
                    Description = "Doesn't have to be M&S ones; I'm open to other brands!",
                    Url = "https://www.ocado.com/products/m-collection-belgian-milk-drinking-chocolate/671529011"
                },
                new Beacon
                {
                    Title = "Peppermint Hot Chocolate",
                    Description = "Doesn't have to be Hotel Chocolate ones; I'm open to other brands!",
                    Url = "https://www.hotelchocolat.com/uk/peppermint-hot-chocolate.html"
                },
                new Beacon
                {
                    Title = "Hotel Chocolat Variety Sachets",
                    Description = "Given it's their variety pack, guess this one does need to be from here...",
                    Url = "https://www.hotelchocolat.com/uk/hot-chocolate-variety-pack.html?cgid=drinking-chocolate"
                },
                new Beacon
                {
                    Title = "Hotel Chocolat Cold Banana Sachets",
                    Description = "'Cold' chocolate sounds intriguing; never had it before!",
                    Url = "https://www.hotelchocolat.com/uk/banana-cold-drinking-chocolate.html"
                },
                new Beacon
                {
                    Title = "Knoops Chocolate Honeycomb Flakes",
                    Description = "FYI: seems to have a slow delivery time.",
                    Url = "https://knoops.com/uk/collections/chocolate/products/41-milk-chocolate-honeycomb"
                },
                new Beacon
                {
                    Title = "Knoops Chocolate Flakes",
                    Description = "FYI: seems to have a slow delivery time.",
                    Url = "https://knoops.com/uk/collections/chocolate/products/milk-chocolate-flakes"
                },
                new Beacon
                {
                    Title = "After Eight Fondants",
                    Description = "Naturally, doesn't have to be from Ocado!",
                    Url = "https://www.ocado.com/products/after-eight-dark-mint-chocolate-fondants/615060011"
                },
                new Beacon
                {
                    Title = "After Eight Thins",
                    Description = "Naturally, doesn't have to be from Ocado!",
                    Url = "https://www.ocado.com/products/after-eight-mint-chocolate-thins-box/11367011"
                },
                new Beacon
                {
                    Title = "M&S Lightly Salted Caramels",
                    Description = "Naturally, doesn't have to be from Ocado!",
                    Url = "https://www.ocado.com/products/m-collection-lightly-salted-caramels/512533011"
                },
                new Beacon
                {
                    Title = "Terry's Chocolate Mint Crisp",
                    Description = "Naturally, doesn't have to be from Ocado!",
                    Url = "https://www.ocado.com/products/terry-minis-mint-crisp/640734011"
                }
            ]
        },
        new BeaconCategoryItems
        {
            Name = "Care",
            Beacons = [
                new Beacon
                {
                    Title = "Monte & Wilde",
                    Description = "This can be either the roll-ons or the spray (linked). Any of the three colours is fine!",
                    Url = "https://www.marksandspencer.com/search?searchTerm=monte+and+wilde&filter=Categories%253DSC_Level_1_462464"
                },
                new Beacon
                {
                    Title = "Oral-B iO Heads (Black)",
                    Description = "Please ensure it's black as that be my toothbrush colour!",
                    Url = "https://www.amazon.co.uk/Oral-B-Ultimate-Clean-Toothbrush-Counts/dp/B089TQLLCQ/ref=sr_1_8?crid=H3LL97F792SP&dib=eyJ2IjoiMSJ9.DYm2vsvtJ0DYzV2GjQqfbJzi2eFuIuilgzlKavAdkDTV9HEPcRbbxk29hB7d9Hd2rko565KEBcxQaCqORSXHHtSkqqYZabF-Ehwm2IR8sMyn5p2yjbJFxNNG5c0JqoCEmbbA2J5c7AdJDKlnYS8B-a0Qore2OiUY2iELELqO5I2yTGvLPeXFMSz1ohV2iTstLy8rrjxUxWTX5xJD2rL3VY_rNTM-Vnm0l3HwyuZo2D68v9jDbZyBQHvesbFx9pZHa-o2zKnIiGlUbjy1HRisAMLlc3HKcswwg7U845U2KM8.s4iwPjWnMRX1ciu39ZTAZcSCLCjFcmYmrushb8mDdV8&dib_tag=se&keywords=oral%2Bb%2BiO%2Btoothbrush%2Bhead&qid=1762107390&sprefix=oral%2Bb%2Bio%2Btoothbrush%2Bhead%2Caps%2C119&sr=8-8&th=1"
                }
            ]
        },
        new BeaconCategoryItems
        {
            Name = "Misc",
            Beacons = [
                new Beacon
                {
                    Title = "M&S Gift Card",
                    Description = "If all else fails, a gift card (or money) will do plenty!",
                    Url = "https://www.marksandspencer.com/l/gifts/gift-cards#intid=gnav_gifts_core_all-gift-cards_gift-cards"
                },
                new Beacon
                {
                    Title = "Donate to the AFU 💛💙",
                    Description = "Or any other Ukrainian charity. They need your offerings more than I do...",
                    Url = "https://www.help99.co"
                }
            ]
        }
    ];
}
