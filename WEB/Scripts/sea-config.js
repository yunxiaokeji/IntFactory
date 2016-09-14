
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
        "form": "/Scripts/jquery.form.js",
        "parser": "/Scripts/jquery.parser.js",
        //颜色选择器
        "color": "plug/choosecolor/spectrum.js",
        "colormark": "plug/colormark/colormark.js",
        //HTML模板引擎
        "dot": "plug/doT.js",
        //分页控件
        "pager": "plug/datapager/paginate.js",
        //报表底层
        'zrender': 'plug/echarts/zrender/zrender.js',
        //排序
        "sortable": "plug/sortable.js",
        //日期控件
        'moment': 'plug/daterangepicker/moment.js',
        'daterangepicker': 'plug/daterangepicker/daterangepicker.js'
    },
    map: [
        //可配置版本号
        ['.css', '.css?v=20160920'],
        ['.js', '.js?v=20160920']
    ]
});


seajs.config({
    alias: {
        //数据验证
        "verify": "plug/verify.js",
        //城市地区
        "city": "plug/city.js",
        //日志
        "logs": "plug/logs/logs.js",
        //备忘
        "replys": "plug/replys/replys.js",
        //上传
        //"upload": "plug/upload/upload.js",
        "upload": "plug/qiniustorage/qiniu/qiniu.js",
        //开关插件
        "switch": "plug/switch/switch.js",
        //标签插件
        "mark": "plug/mark/mark.js",
        //弹出层插件
        "easydialog": "plug/easydialog/easydialog.js",
        //搜索插件
        "search": "plug/seach_keys/seach_keys.js",
        //购物车
        "cart": "plug/shoppingcart/shoppingcart.js",
        //选择员工
        "chooseuser": "plug/chooseuser/chooseuserjs.js",
        //选择客户
        "choosecustomer": "plug/choosecustomer/choosecustomer.js",
        //选择工厂
        "choosefactory": "plug/choosefactory/choosefactory.js",
        //选择打样单
        "chooseorder": "plug/chooseorder/chooseorder.js",
        //选择流程
        "chooseprocess": "plug/chooseprocess/chooseprocess.js",
        //选择产品
        "chooseproduct": "plug/chooseproduct/chooseproduct.js",
        //选择下属
        "choosebranch": "plug/choosebranch/choosebranch.js",
        //下拉框
        "dropdown": "plug/dropdown/dropdown.js",
        //搜索下拉框
        "autocomplete": "plug/autocomplete/autocomplete.js",
        //显示用户名片层
        "businesscard": "plug/businesscard/businesscard.js",
        //分享明道
        "sharemingdao": "plug/sharemingdao/sharemingdao.js",
        //日程列表
        "fullcalendar": "plug/fullcalendar/fc.js",
        //显示任务详情
        "showtaskdetail": "plug/showtaskdetail/showtaskdetail.js",
        //显示讨论表情插件
        "qqface": "plug/qqface/qqface.js",
        //显示提示插件
        "tip": "plug/tip/tip.js",
        //选择分类插件
        "menu": "plug/choosemenu/choosemenu.js",
        //快速添加材料插件
        "quicklyproduct": "plug/quicklyaddproduct/quickaddproduct.js",
        //显示明细弹出层
        "detaillayer": "plug/detaillayer/detaillayer.js",
        //提示浮层插件
        "tiplayer": "plug/tip_layer/tip_layer.js"
    }
});
