function getCookie(name) {
    var cookieName = encodeURIComponent(name) + "=";
    var cookie = document.cookie;
    var value = null;

    var startIndex = cookie.indexOf(cookieName);
    if (startIndex > -1) {
        var endIndex = cookie.indexOf(';', startIndex);
        if (cookie == -1) {
            endIndex = cookie.length;
        }
        value = decodeURIComponent(cookie.substring(startIndex + name.length, endIndex));
    }

    return value.substring(1);
}

function setCookie(name, value) {
    var cookieText = encodeURIComponent(name) + "=" + encodeURIComponent(value);
    cookieText += "; expires=" + new Date(new Date().setFullYear(new Date().getFullYear() + 1)).toGMTString();
    cookieText += "; domain=" + window.location.hostname;
    document.cookie = cookieText;
}

$(document).ready(function() {

    // Add language-none class to code blocks with no language specified.
    $("pre:not([class]) > code:not([class])").parent().addClass("language-none");

    // language-none blocks that have content starting with "<" are likely UX blocks.
    $("pre.language-none > code").each(function() {
        var block = $(this);
        if (block.text().substring(0, 1) === "<") {
            block.parent().removeClass("language-none").addClass("language-ux");
            Prism.highlightElement(block[0]);
        }
    });

    // Add links to headers with anchors/ids on them.
    var headers = $(".main-content h1[id], .main-content h2[id], .main-content h3[id], .main-content h4[id], .main-content h5[id], .main-content h6[id]");
    headers.each(function() {
        var heading = $(this);
        var id = heading.attr('id');
        heading.addClass('anchor-header');

        var anchor = $('<a><i class="fa fa-link"></i></a>').attr('href', '#' + id).attr('aria-hidden', 'true').addClass('anchor-header-link');
        heading.prepend(anchor);
    });

    docsearch({
            apiKey: '37ca02a1d0b845d74a74a48683774e97',
            indexName: 'fuse',
            inputSelector: '#search-input',
            debug: false // Set debug to true if you want to inspect the dropdown
            });

    $("#search-input").on("input", function (event) {
        $(".sticky").css({overflow: 'unset'})
        $(".main-content").css({zIndex: '-1'})
    })
    $("#search-input").blur(function (event) {
        $(".sticky").css({overflow: 'auto'})
        $(".main-content").css({zIndex: 'unset'})
    })

    var persistedShow = getCookie("advanced-items");
    $('.is-advanced').css('display', persistedShow);
    $('.only-advanced-items').css('display', persistedShow);

    $(".advance-items").change(function() {
        var show = 'none';
        if (this.checked) {
            show = 'block';
        }
        $('.is-advanced').css('display', show);
        $('.only-advanced-items').css('display', show);
        setCookie("advanced-items", show);
    });
});
