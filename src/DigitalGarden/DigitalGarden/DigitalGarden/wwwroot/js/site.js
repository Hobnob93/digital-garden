$(window).on('scroll', function () {
    const y = $(this).scrollTop();
    $('nav').toggleClass('affix', y > 50);
});

function initializeMouseOverCards(cardsRootElement) {
    $(cardsRootElement).on('mousemove', function (event) {
        for (const card of $(this).children('.card')) {
            const rect = card.getBoundingClientRect(),
                x = event.clientX - rect.left,
                y = event.clientY - rect.top;

            card.style.setProperty('--mouse-x', `${x}px`);
            card.style.setProperty('--mouse-y', `${y}px`);
        };
    });

    cardsRootElement.addEventListener('wheel', (event) => {
        if (event.deltaY === 0)
            return;

        event.preventDefault();
        cardsRootElement.scrollBy({ left: event.deltaY, behavior: 'smooth' });
    }, { passive: false });
}
