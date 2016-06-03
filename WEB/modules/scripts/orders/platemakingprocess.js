define(function (require,exports,module) {

    var Objects = {};

    Objects.init = function () {
        Objects.bindEvent();
    };

    Objects.bindEvent = function () {
        
        alert("hello");
       
    };

    module.exports = Objects;
});