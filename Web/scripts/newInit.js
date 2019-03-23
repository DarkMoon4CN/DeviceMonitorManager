$(function () {
    BindRightAccordion();
});
function BindRightAccordion() {
    $("#RightAccordion").accordion({ //初始化accordion
        fillSpace: true,
        fit: true,
        border: false,
        animate: false
    });

    $.post("/Home/GetTreeByEasyui", { "parentId": "0" }, function (data) {
          console.log(data);
    });
    
}