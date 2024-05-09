//Part Home
const prev = document.querySelector('.slider-prev');
const next = document.querySelector('.slider-next');
const sliderImage = document.querySelectorAll('.sliders img')
const sliderContainer = document.querySelector('.slider-container')
let autoSlideInterval;
let counter = 0;

function sliderNext() {
    sliderImage[counter].style.animation = 'next1 0.5s ease-in forwards'
    if (counter >= sliderImage.length - 1) {
        counter = 0;
    }
    else {
        counter++;
    }
    sliderImage[counter].style.animation = 'next2 0.5s ease-in forwards'
}
function sliderPrev() {
    sliderImage[counter].style.animation = 'prev1 0.5s ease-in forwards'
    if (counter == 0) {
        counter = sliderImage.length - 1
    }
    else {
        counter--;
    }
    sliderImage[counter].style.animation = 'prev2 0.5s ease-in forwards'
}

function autoSliding() {
    deleInterval = setInterval(timer, 2000)
    function timer() {
        sliderNext()
    }
}
sliderContainer.addEventListener('mouseover', function () {
    clearInterval(deleInterval)
})
sliderContainer.addEventListener('mouseout', autoSliding)
next.addEventListener('click', sliderNext)
prev.addEventListener('click', sliderPrev)
autoSliding()