
/*!
 * 菜单切换
 * 2013/12/4
 */

$().ready(function () {

    var cookieName = "$safeprojectname$UserMenuV";
	var ulMenuObj = $("#accordion");
	var
		obj = $(ulMenuObj).find("li"),  // 父菜单
		menuObj = $(ulMenuObj).find("ul");  // 主菜单

	// 父菜单
	$(obj).on("click", function (e) {
	    var event = e || window.event;
	    var target = event.srcElement || event.target;
		var self = $(this);
		var index = self.index();

	    //slideup or hide all the Submenu
		if ($(target).attr("class") == "buy") {

		    $(ulMenuObj).find('li').children('ul').slideUp('fast');
		}

		//remove all the "Over" class, so that the arrow reset to default
		$(ulMenuObj).find('li > a').each(function () {
			if ($(this).attr('rel') != '') {
				$(this).removeClass($(this).attr('rel') + 'Over');
			}
		});

	    //show the selected submenu
		if ($(target).attr("class") == "buy") {
		    $(this).children('ul').slideDown('fast');
		}

		//add "Over" class, so that the arrow pointing down
		$(this).children('a').addClass($(this).children('li a').attr('rel') + 'Over');

		// 保存父菜单索引
		$.cookie(cookieName, index + "-0", { path: '/' });

	})

	var cookieValue = $.cookie(cookieName);

	// 当Cookie值为空时，默认第一个
	if (cookieValue === "" || cookieValue === null || typeof cookieValue === "undefined") {
		// 显示对应第一个
		$(menuObj.eq(0)).show();
		$(obj.eq(0)).children('a').addClass($(obj.eq(0)).children('li a').attr('rel') + 'Over');
	}
	// 找到Cookie 值初始化之前的页面
	if (cookieValue !== "" && typeof cookieValue !== "undefined" && cookieValue != null) {
		var pIndex = cookieValue.split("-")[0];
		var cIndex = cookieValue.split("-")[1];
		$(menuObj).hide(); // 隐藏所有模块
		$(menuObj).eq(pIndex).show();  // 显示当前 模块
	}

})
