
/*基础配置*/
seajs.config({
    base: "/modules/",
    paths: {
        "echarts": 'plug/echarts/',
        "zrender": 'plug/echarts/zrender/'
    },
    alias: {
        "jquery": "/Scripts/jquery-1.11.1.js",
        "global": "scripts/global.js",
        //HTML模板引擎
        "dot": "plug/doT.js",
        //分页控件
        "pager": "plug/datapager/paginate.js",
        //报表底层
        'zrender': 'plug/echarts/zrender/zrender.js'
    }
});


seajs.config({
    alias: {
        //数据验证
        "verify": "plug/verify.js",
        //城市地区
        "city": "plug/city.js",
        //搜索插件
        "search": "plug/seach_keys/seach_keys.js",
        //下拉框
        "dropdown": "plug/dropdown/dropdown.js",
        //弹出层插件
        "easydialog": "plug/easydialog/easydialog.js"
    }
});

