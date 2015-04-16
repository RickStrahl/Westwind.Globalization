/// <reference path="jquery.js" />
/// <reference path="ww.jquery.js" />
(function () {
    if (typeof (ww) == 'undefined')
        ww = {};
    var self = null;

    ww.resourceEditor = {
        showResourceIcons: function (options) {
            self.removeResourceIcons();
            
            var opt = {
                adminUrl: "LocalizationAdmin/"
            };
            $.extend(opt, options);

            var set = $("[data-resource-set]");
            if (set.length < 1) {
                console.log("resourceEditor: No 'data-resource-set' attribute defined");
                return;
            }
            var resourceSet = set.data("resource-set");

            var $els = $("[data-resource-id");
            if ($els.length < 1) {
                console.log("resourceEditor: No 'data-resource-id' attributes found");
                return;
            }

            $els.each(function() {
                var $el = $(this);
                var resId = $el.data("resource-id");
                var resSet = $el.data("resource-set");

                var content = $el.text() || $el.val();
                
                if (content && content.length > 600)
                    content = "";
                
                if (!resSet) {
                    var $resSets = $el.parents("[data-resource-set]");                    
                    if ($resSets.length > 0)
                        resSet = $resSets.eq(0).data("resource-set");
                }
                var pos = $el.position();

                console.log("Url: " + encodeURIComponent(resSet)); 
                var $new = $("<res-edit>")
                    .addClass("resource-editor-icon")
                    .css(pos)
                    .attr("target", "resourceEditor")
                    .attr("title","Edit resource: " + resId )
                    .attr("onclick","window.open('" + opt.adminUrl + "?ResourceSet=" + resSet + "&ResourceId=" +  resId + "&Content=" +  encodeURIComponent(content) + "','resourceEdit')");
                    
                $new.insertBefore($el);

            });

            $(window).resize(function() {
                ww.resourceEditor.removeResourceIcons();
                ww.resourceEditor.showResourceIcons(options);
            });
        },
        removeResourceIcons: function() {
            $(".resource-editor-icon").remove();
        },
        isResourceEditingEnabled: function() {
            if ($(".resource-editor-icon").length > 0)
                return true;
            return false;
        }

    };

    self = ww.resourceEditor;
})();

(function() {
    if (typeof (ww) == 'undefined')
        ww = {};
    var self = null;
    ww.resourceEditorControl = {
        
    };

    self = ww.resourceEditorControl;
})();
    