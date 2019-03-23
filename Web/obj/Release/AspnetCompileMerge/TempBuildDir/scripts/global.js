var Global = {
    Init: function () { }
};
Global.PostData;
Global.Success = true;
Global.Post = function (url, data, callback) {
    try {
        $.ajax({
            url: url,
            type: 'POST',
            data: data,
            async: false,
            error: function (e) {
                Global.Success = false;
                layer.msg("与服务器中断了链接...", { time: 2000 });
            },
            success: Global.PostCallback
        });
    } catch (e) {
        Global.PostData = null;
        Global.Success = false;
    }
}
Global.PostCallback = function (data) {
    Global.PostData = null;
    Global.PostData = data;
}

Global.Base64=function(input_file, get_data) {
    /*input_file：文件按钮对象*/
    /*get_data: 转换成功后执行的方法*/
    if (typeof (FileReader) === 'undefined') {
        layer.msg("抱歉，你的浏览器不支持 FileReader，不能将图片转换为Base64，请使用IE 8以上浏览器操作！", { time: 1000 });
    } else {
        try {
            /*图片转Base64 核心代码*/
            var file = input_file.get(0).files[0];
            var reader = new FileReader();
            reader.onload = function () {
                get_data(this.result);
            }
            reader.readAsDataURL(file);
        } catch (e) {
            alert('图片转Base64出错啦！' + e.toString())
        }
    }
}

Global.GetFileService = function ()
{
    var url = "/Main/GetFileService";
    var data = {};
    Global.Post(url, data, Global.PostCallback);
    return Global.PostData.Data;
}

Global.OldUpLoad = function (fileByte, extName, fileType)
{
    //所有压力放置在客户端
    var fileServicePath = Global.GetFileService();
    Global.Base64($(this), function (data) {
        var mydata = { fileByte: data, extName: extName, fileType: 1 };
        Global.Post(fileServicePath, mydata, Global.PostCallback);
        return Global.PostData;
    })
}
