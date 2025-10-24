$(document).on('mousemove', function (e) {
    document.documentElement.style.setProperty('--mouse-x', e.pageX + 'px');
    document.documentElement.style.setProperty('--mouse-y', e.pageY + 'px');
});
