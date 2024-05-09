
const tab = $('#navbar-tab-menu')
window.onscroll = function (e) {
    let scrollValue = document.documentElement.scrollTop
    if (scrollValue >= 0 && scrollValue <= 66) {
        tab.css({ 'top': `calc(168px - ${scrollValue}px)` })
    }
    else if (scrollValue > 66) {
        tab.css({ 'top': `103px` })
    }
}
// Active Tab Menu
$('.nav-tab-menu-link').on('click', function () {
    $('.nav-tab-menu-link').removeClass('activer');
    $(this).addClass('activer');
});