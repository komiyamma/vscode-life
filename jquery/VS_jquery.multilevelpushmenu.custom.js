var menuMoveMode = false;
var isViewingPageLoading = false; // ページロードの表示中

$(document).ready(function(){
  $( '#eventpanel' ).empty();

  $( '#menu' ).multilevelpushmenu({
    containersToPush: [$( '#pushobj' )] ,
    menuWidth: 380,
    menuHeight: $(window).height(), // 左側のメニューの高さは最低1500
    overlapWidth: 56,

    onBackItemClick: function() {
        menuMoveMode = true;
    },
    onGroupItemClick: function() {
        menuMoveMode = true;
    },

    onCollapseMenuEnd: function() {
        if ( menuMoveMode == true ) {
            if ( ! $(location).attr('href').match("#") ) {
                var speed = 100;
                $('body,html').animate({scrollTop:0}, speed, 'swing');
            }
        }
        menuMoveMode = false;
    },
    onExpandMenuEnd: function() {
        if ( menuMoveMode == true ) {
            if ( ! $(location).attr('href').match("#") ) {
                var speed = 100;
                $('body,html').animate({scrollTop:0}, speed, 'swing');
            }
        }
        menuMoveMode = false;
    },

    onItemClick: function() {
        var event        = arguments[0],
        $menuLevelHolder = arguments[1],
        $item            = arguments[2],
        options          = arguments[3],
        title = $menuLevelHolder.find( 'h2:first' ).text(),
        itemName = $item.find( 'a:first' ).text(),
        itemHref = $item.find( 'a' ).attr("href");

        if ( itemHref != "#" ) {
            if ( !isViewingPageLoading && !itemHref.match("http") ) {
                isViewingPageLoading = true;
                $( '#eventpanel' ).append( getPageLoadString() );
            }

            menuMoveMode = false;
            window.location = itemHref;
        }
    },
  });

  doMenuExpand();
  isViewed = true;
});

function doMenuAdjustSize() {
    var contents_height = parseInt($( '#pushobj' ).css( 'height' )) + 100;
    var window_height = $(window).height();
    contents_height = contents_height > window_height ? contents_height : window_height;
    contents_height = contents_height > 1500 ? contents_height : 1500;
    if ( $( '#menu' ).height() != contents_height ) {
        $( '#menu' ).multilevelpushmenu( 'option', 'menuHeight', contents_height );
        $( '#menu' ).multilevelpushmenu( 'redraw' );
    }
}

function getPageLoadString() {
    return '<div class="divPageLoad" style="margin:3px">&nbsp;<i class="fa fa-refresh fa-spin"></i>&nbsp;ページ読み込み中です...</div>';
}

$(window).on("load", function () {
    // 1秒後にリサイズ
    setTimeout(function () {
        doMenuAdjustSize();
    }, 1000);

    // 3秒後に再びリサイズ
    setTimeout(function () {
        doMenuAdjustSize();
    }, 3000);
});

$(window).on("resize", function () {
    // 2秒後に再びリサイズ
    setTimeout(function () {
        doMenuAdjustSize();
    }, 2000);
});

$(window).on("beforeunload", function (e) {
    $("divPageLoad").remove();
});
