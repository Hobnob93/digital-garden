$(window).on('scroll', function () {
    const y = $(this).scrollTop();
    $('nav').toggleClass('affix', y > 50);
});
