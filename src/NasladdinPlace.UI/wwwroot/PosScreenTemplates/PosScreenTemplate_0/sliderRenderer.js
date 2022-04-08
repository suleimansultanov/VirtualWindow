const SliderRenderer = function() {
    const render = function(element) {
        if (element === undefined) {
            return;
        }

        $(element).slick({
            slidesToShow: 1,
            slidesToScroll: 1,
            autoplay: true,
            autoplaySpeed: 10000,
            dots: false,
            infinite: true,
            arrows: false,
            draggable: false,
            swipe: false
        });
    };

    return {
        render: render
    };
}();