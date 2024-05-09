//Phần show and hidden button cart
const btn_header = document.querySelector('.btn_dathang--header')
const hadleScroll = () => {
    if (window.scrollY >= 20) {
        Object.assign(btn_header.style, {
            opacity: 1,
            PointerEvent: 'auto',
        })
        btn_header.classList.remove("disabled")
    }
    else {
        Object.assign(btn_header.style, {
            opacity: 0,
            PointerEvent: 'none',
        })
        btn_header.classList.add("disabled")
    }
}
window.addEventListener('scroll', hadleScroll)

//Part Modal-Menu
const wrappermodal = $('.wrapper-menu-modals')
$('.close').click(function () {
    $('.menu-modals').css({ 'display': 'none' })
    $('.menu-modals-inner').css({ 'right': '-350px' })
})
wrappermodal.click(function () {
    $('.menu-modals-inner').css({ 'right': '0' })
    $('.menu-modals').toggle("slow")
})

