window.aceEditors = {
    editors: {},

    create: function (editorId, theme, mode,value,isReadOnly) {
        const editor = ace.edit(editorId);
        editor.setTheme("ace/theme/" + theme);
        editor.session.setMode("ace/mode/" + mode);
        editor.setOption("showPrintMargin", false);
        editor.session.setUseWrapMode(true);
        editor.session.setWrapLimitRange(120, 120);
        editor.setReadOnly(isReadOnly);
        editor.setValue(value, -1);
        
        this.editors[editorId] = editor;
    },

    getValue: function (editorId) {
        const editor = this.editors[editorId];
        return editor ? editor.getValue() : "";
    },

    setValue: function (editorId, value) {
        const editor = this.editors[editorId];
        if (editor) editor.setValue(value, -1);
    },

};
