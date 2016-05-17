

var timeout = 500;
var closetimer = 0;
var ddmenuitem = 0;

function jsddm_open() {
    jsddm_canceltimer();
    jsddm_close();
    ddmenuitem = $(this).find('ul').eq(0).css('visibility', 'visible');
}

function jsddm_close()
{ if (ddmenuitem) ddmenuitem.css('visibility', 'hidden'); }

function jsddm_timer()
{ closetimer = window.setTimeout(jsddm_close, timeout); }

function jsddm_canceltimer() {
    if (closetimer) {
        window.clearTimeout(closetimer);
        closetimer = null;
    }
}

function isHTML(content){
    var result = content.match(/<\w+\s*>/);
    return result != null;
}

function hideHTML(element) {
    $(element).children().each(function (i, e) {
        if (i > 0 && !$(e).hasClass('show')) {
            $(e).hide();
        }
    });
}

$(document).ready(function () {
    $('.menu-s > li').bind('mouseover', jsddm_open);
    $('.menu-s > li').bind('mouseout', jsddm_timer);
});

document.onclick = jsddm_close;

$(document).ready(function () {
    $('ul.cities-s').children('li:gt(4)').hide();
    $('.btn_less').hide();

    $('.btn_more').click(function () {
        $('ul.cities-s').children().show();
        $('.btn_more').hide();
        $('.btn_less').show();
    });
    $('.btn_less').click(function () {
        $('ul.cities-s').children('li:gt(4)').hide();
        $('.btn_less').hide();
        $('.btn_more').show();
    });

});

$(document).ready(function () {
    $(".minimize").click(function () {

        $(".ads-s").slideUp(500);
        $(".minimize").css("display", "none");
        $(".maximize").css("display", "block");
    });
    $(".maximize").click(function () {
        $(".ads-s").slideDown(500);
        $(".maximize").css("display", "none");
        $(".minimize").css("display", "block");
    });
});


$(document).ready(function () {
    var showChar = 200;
    var ellipsestext = "...";
    var moretext = "Read More";
    var lesstext = "Read Less";

    $('.more').each(function () {
        var content = $(this).html();

        if (content.length > showChar) {
            if (!isHTML(content)) {                
                var c = content.substr(0, showChar);
                var h = content.substr(showChar, content.length - showChar);
                var html = c + '<span class="moreelipses">' + ellipsestext + '</span><span class="morecontent"><span style="display:none;">' + h + '</span>&nbsp;&nbsp;<a href="" class="morelink">' + moretext + '</a></span>';
                $(this).html(html);
            }
            else {
                var html = $(this).html() + '<p class="show"><a href="" class="showrest">' + moretext + '</a></p>';
                $(this).html(html);
                hideHTML(this);
            }
        }

    });

    $(".morelink").click(function () {
        if ($(this).hasClass("less")) {
            $(this).removeClass("less");
            $(this).html(moretext);
        } else {
            $(this).addClass("less");
            $(this).html(lesstext);
        }
        $(this).parent().prev().toggle();
        $(this).prev().toggle();
        return false;
    });

    $('.showrest').click(function () {
        if ($(this).hasClass("less")) {
            $(this).removeClass("less");
            $(this).html('<p class="show"><a href="" class="showrest">' + moretext + '</a></p>');
            hideHTML($(this).parents('.more'));
        } else {
            $(this).addClass("less");
            $(this).html('<p class="show"><a href="" class="showrest">' + lesstext + '</a></p>');
            $(this).parents('.more').children().show();
        }
        return false;
    });
});


$(function () {
    $('.contact').click(function () {
        $('.contact-form').show();
        return false;
    });
    $('.close').click(function () {
        $('.contact-form').hide();
        return false;
    });
});


$(document).ready(function () {
    $("#right").click(function () {
        var selectedItem = $("#leftvalues option:selected");
        $("#rightvalues").append(selectedItem);
        $("#rightvalues option").attr("selected", false);
        var options = $('#rightvalues option');
        var arr = options.map(function (_, o) { return { t: $(o).text(), v: o.value }; }).get();
        arr.sort(function (o1, o2) { return o1.t > o2.t ? 1 : o1.t < o2.t ? -1 : 0; });
        options.each(function (i, o) {
            o.value = arr[i].v;
            $(o).text(arr[i].t);
        });
        
    });

    $("#left").click(function () {
        var selectedItem = $("#rightvalues option:selected");
        $("#leftvalues").append(selectedItem);
        $("#leftvalues option").attr("selected", false);
        var options = $('#leftvalues option');
        var arr = options.map(function (_, o) { return { t: $(o).text(), v: o.value }; }).get();
        arr.sort(function (o1, o2) { return o1.t > o2.t ? 1 : o1.t < o2.t ? -1 : 0; });
        options.each(function (i, o) {
            o.value = arr[i].v;
            $(o).text(arr[i].t);
        });
    });

    $('#leftvalues').dblclick(function () {
        $('#right').trigger('click');
    });

    $('#rightvalues').dblclick(function () {
        $('#left').trigger('click');
    });

});


$(document).ready(function () {
    $('.expander').addClass('expand');

    $(".expander").click(function () {
        if ($(this).hasClass('expand')) {
            $(this).removeClass('expand');
            $(this).addClass('minimize');
        }
        else {
            $(this).removeClass('minimize');
            $(this).addClass('expand');
        }        

        $(".panel-cont").toggle();
    });

    $(".filters h4").click(function () {
        $(".filter-fields").toggle();
    });

    $('.radio label').click(function () {
        $(this).children("input").prop("checked", true);
    });

    $('.characterCount').each(function (i, e) {
        $(this).height($(this).prev().height());
        $(this).children("label").css("position", "absolute").css("bottom", 0);
    });    
});