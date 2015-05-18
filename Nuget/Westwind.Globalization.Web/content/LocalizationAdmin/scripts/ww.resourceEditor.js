/// <reference path="jquery.js" />
/// <reference path="ww.jquery.js" />
(function () {
    if (typeof (ww) == 'undefined')
        ww = {};
    var self = null;

    ww.resourceEditor = {
        options: null,
        showResourceIcons: function (options) {
            self.removeResourceIcons();

            var opt = {
                adminUrl: "LocalizationAdmin/",
                editorWindowOpenOptions: "", // "height=600, width=900, left=30, top=30",
                editorWindowName: "_localization-resource-editor"
            };
            $.extend(opt, options);
            self.options = opt;

            var set = $("[data-resource-set]");
            if (set.length < 1) {
                console.log("resourceEditor: No 'data-resource-set' attribute defined");
                return;
            }            

            var $els = $("[data-resource-id");
            if ($els.length < 1) {
                console.log("resourceEditor: No 'data-resource-id' attributes found");
                return;
            }

            $els.each(function() {
                var $el = $(this);
                var resId = $el.data("resource-id");      
                var pos = $el.position();
                
                var $new = $("<res-edit>")
                    .addClass("resource-editor-icon")
                    .css(pos)
                    .data("resource-element",this) // store actual base element
                    .attr("target", "resourceEditor")
                    .attr("title", "Edit resource: " + resId)
                    .click(self.showEditorForm);
    
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
        },
        showEditorForm: function (e) {

            e.preventDefault();

            var $el = $($(this).data("resource-element"));
            var resId = $el.data("resource-id");
            var resSet = $el.data("resource-set");
            var content = $el.text() || $el.val() || "";
            content = $.trim(content);

            if (content && content.length > 600)
                content = "";

            if (!resSet) {
                var $resSets = $el.parents("[data-resource-set]");
                if ($resSets.length > 0)
                    resSet = $resSets.eq(0).data("resource-set");
            }
            
            window.open(self.options.adminUrl + "?ResourceSet=" + encodeURIComponent(resSet) +
                    "&ResourceId=" + encodeURIComponent(resId) +
                    "&Content=" + encodeURIComponent(content),
                    self.options.editorWindowName, self.options.editorWindowOpenOptions);
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
    