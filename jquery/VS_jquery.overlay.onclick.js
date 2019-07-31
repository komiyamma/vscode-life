$(function() {
    $('body').prepend('<div class="overlay"></div>');
    $('img.lazy').click(function() {
        var left = ($(window).width() / 2) + $(window).scrollLeft() - ($(this).attr('width') / 2);
        var top = ($(window).height() / 2) + $(window).scrollTop() - ($(this).attr('height') / 2);
        $('div.overlay').css('height', $(document).height());
        $('div.overlay').hide().empty();
        $('div.overlay').append('<img src="' + $(this).attr('data-original') + '" attr="' + $(this).attr('attr') + '" />');
        $('div.overlay img').css({left: left, top: top, opacity: '1'});
        $('div.overlay').fadeIn("fast");
        return false;
    });
    $('div.overlay').click(function() {
        $('div.overlay').fadeOut("fast");
    });
});