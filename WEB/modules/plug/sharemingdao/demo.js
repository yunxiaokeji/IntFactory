define(function (require, exports, module) {
    var ObjectJS = {};

    ObjectJS.init = function (options) {
        require.async("sharemingdao", function () {
            $("#demo").sharemingdao(options);
        });
    };



    module.exports = ObjectJS;
});