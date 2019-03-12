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

    // Adds a CSS class to a blockquote tag that has a marker like [callout_warn]
    $("blockquote:contains('[callout_warn]')").addClass("callout-warn");
    $("blockquote:contains('[callout_warn]')").html($("blockquote:contains('[callout_warn]')").html().replace('[callout_warn]',''));
    $("blockquote:contains('[callout_success]')").addClass("callout-success");
    $("blockquote:contains('[callout_success]')").html($("blockquote:contains('[callout_success]')").html().replace('[callout_success]',''));
    $("blockquote:contains('[callout_danger]')").addClass("callout-danger");
    $("blockquote:contains('[callout_danger]')").html($("blockquote:contains('[callout_danger]')").html().replace('[callout_danger]',''));
    $("blockquote:contains('[callout_info]')").addClass("callout-info");
    $("blockquote:contains('[callout_info]')").html($("blockquote:contains('[callout_info]')").html().replace('[callout_info]',''));
    $("blockquote:contains('[callout_light]')").addClass("callout-light");
    $("blockquote:contains('[callout_light]')").html($("blockquote:contains('[callout_light]')").html().replace('[callout_light]',''));
    $("blockquote:contains('[callout_dark]')").addClass("callout-dark");
    $("blockquote:contains('[callout_dark]')").html($("blockquote:contains('[callout_dark]')").html().replace('[callout_dark]',''));
    
});
