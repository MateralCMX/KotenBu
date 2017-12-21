/// <reference path="../lib/m-tools/m-tools.ts" />
'use strict';
import MDMa = MateralTools.DOMManager;
import MTMa = MateralTools.ToolManager;
import MLDMa = MateralTools.LocalDataManager;
namespace KotenBu {
    class Common {
        /**
         * 构造方法
         */
        constructor() {
            if (performance.navigation.type != 2) {
                ApplicationSettingModel.IsDebug = true;
                this.AddMessageBoxArticle();
                this.BindEvent();
            }
            else {
                window.location.reload();
            }
        }
        /**
         * 绑定事件
         */
        private BindEvent() {
            MDMa.AddEvent(window, "keypress", this.KeyEnterCloseMessageBoxEvent);
        }
        /*应用程序配置对象*/
        public ApplicationSettingM: ApplicationSettingModel = new ApplicationSettingModel();
        /*分页配置*/
        public PagingM = {
            PageShowBtnNum: 9,
            PageModel: {
                DataCount: 0,
                PagingCount: 99,
                PagingIndex: 1,
                PagingSize: 10
            } as MPagingModel
        };
        /**
         * 绑定分页信息
         * @param pageM 分页信息对象
         */
        public BindPageInfo(pageM: MPagingModel, getList: Function): void {
            let PageInfo = MDMa.$("PageInfo");
            if (!MTMa.IsNullOrUndefined(PageInfo)) {
                PageInfo.innerHTML = "";
                let PageBody = document.createElement("ul");
                MDMa.AddClass(PageBody, "pagination");
                /*绑定首页和上一页*/
                let FirstBtn = document.createElement("li");
                let FirstBtnA = document.createElement("a");
                FirstBtnA.setAttribute("title", "首页");
                FirstBtnA.innerHTML = "&laquo;";
                FirstBtn.appendChild(FirstBtnA);
                PageBody.appendChild(FirstBtn);
                let UpBtn = document.createElement("li");
                let UpBtnA = document.createElement("a");
                UpBtnA.setAttribute("title", "上一页");
                UpBtnA.innerHTML = "&lt;";
                UpBtn.appendChild(UpBtnA);
                PageBody.appendChild(UpBtn);
                if (pageM.PagingIndex <= 1) {
                    MDMa.AddClass(FirstBtn, "disabled");
                    MDMa.AddClass(UpBtn, "disabled");
                }
                else {
                    FirstBtn.dataset.index = "1";
                    FirstBtnA.dataset.index = "1";
                    MDMa.AddEvent(FirstBtnA, "click", function (e: MouseEvent) {
                        let index: number = parseInt((e.target as HTMLLIElement).dataset["index"]);
                        common.PagingM.PageModel.PagingIndex = index;
                        getList();
                    });
                    UpBtn.dataset.index = (pageM.PagingIndex - 1).toString();
                    UpBtnA.dataset.index = (pageM.PagingIndex - 1).toString();
                    MDMa.AddEvent(UpBtnA, "click", function (e: MouseEvent) {
                        let index: number = parseInt((e.target as HTMLLIElement).dataset["index"]);
                        common.PagingM.PageModel.PagingIndex = index;
                        getList();
                    });
                }
                /*绑定数字*/
                let Count = 0;
                let index = pageM.PagingIndex - Math.round((common.PagingM.PageShowBtnNum - 1) / 2);
                for (; index <= pageM.PagingCount && Count < common.PagingM.PageShowBtnNum; index++) {
                    if (index >= 1) {
                        let itemBtn = document.createElement("li");
                        let itemBtnA = document.createElement("a");
                        itemBtnA.innerHTML = index.toString();
                        if (index == pageM.PagingIndex) {
                            MDMa.AddClass(itemBtn, "active");
                        }
                        else {
                            itemBtn.dataset.index = index.toString();
                            itemBtnA.dataset.index = index.toString();
                            MDMa.AddEvent(itemBtnA, "click", function (e: MouseEvent) {
                                let index: number = parseInt((e.target as HTMLLIElement).dataset["index"]);
                                common.PagingM.PageModel.PagingIndex = index;
                                getList();
                            });
                        }
                        itemBtn.appendChild(itemBtnA);
                        PageBody.appendChild(itemBtn);
                        Count++;
                    }
                }
                /*绑定下一页和尾页*/
                let NextBtn = document.createElement("li");
                let NextBtnA = document.createElement("a");
                NextBtnA.setAttribute("title", "下一页");
                NextBtnA.innerHTML = "&gt;";
                NextBtn.appendChild(NextBtnA);
                PageBody.appendChild(NextBtn);
                let LastBtn = document.createElement("li");
                let LastBtnA = document.createElement("a");
                LastBtnA.setAttribute("title", "尾页");
                LastBtnA.innerHTML = "&raquo;";
                LastBtn.appendChild(LastBtnA);
                PageBody.appendChild(LastBtn);
                if (pageM.PagingIndex >= pageM.PagingCount) {
                    MDMa.AddClass(NextBtn, "disabled");
                    MDMa.AddClass(LastBtn, "disabled");
                }
                else {
                    NextBtn.dataset.index = (pageM.PagingIndex + 1).toString();
                    NextBtnA.dataset.index = (pageM.PagingIndex + 1).toString();
                    MDMa.AddEvent(NextBtnA, "click", function (e: MouseEvent) {
                        let index: number = parseInt((e.target as HTMLLIElement).dataset["index"]);
                        common.PagingM.PageModel.PagingIndex = index;
                        getList();
                    });
                    LastBtn.dataset.index = pageM.PagingCount.toString();
                    LastBtnA.dataset.index = pageM.PagingCount.toString();
                    MDMa.AddEvent(LastBtnA, "click", function (e: MouseEvent) {
                        let index: number = parseInt((e.target as HTMLLIElement).dataset["index"]);
                        common.PagingM.PageModel.PagingIndex = index;
                        getList();
                    });
                }
                PageInfo.appendChild(PageBody);
                common.PagingM.PageModel = pageM;
            }
        }
        /**
         * 保存查询条件信息
         * @param data
         */
        public SaveSearchInfo(key: string, data: any): void {
            MLDMa.SetSessionData(key, data, true);
        }
        /**
         * 获得查询条件信息
         */
        private GetSearchInfo(key: string): any {
            return MLDMa.SetSessionData(key, true);
        }
        /**
         * 绑定查询信息
         * @param key key
         */
        public BindSearchInfo(key: string) {
            let data = this.GetSearchInfo(key);
            if (!MTMa.IsNullOrUndefined(data)) {
                for (var name in data) {
                    let SearchElement = MDMa.$("Search" + name) as HTMLElement;
                    if (!MTMa.IsNullOrUndefined(SearchElement)) {
                        switch (SearchElement.tagName) {
                            case "INPUT":
                                let InputElement = SearchElement as HTMLInputElement;
                                switch (InputElement.type) {
                                    case "checkbox":
                                        InputElement.checked = data[name];
                                        break;
                                    default:
                                        InputElement.value = data[name];
                                        break;
                                }
                                break;
                            case "SELECT":
                                let SelectElement = SearchElement as HTMLInputElement;
                                SelectElement.value = data[name];
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
        /**
         * 绑定输入信息
         * @param data 数据集
         */
        public BindInputInfo(data: any) {
            if (!MTMa.IsNullOrUndefined(data)) {
                for (var name in data) {
                    let SearchElement = MDMa.$("Input" + name) as HTMLElement;
                    if (!MTMa.IsNullOrUndefined(SearchElement)) {
                        switch (SearchElement.tagName) {
                            case "INPUT":
                                let InputElement = SearchElement as HTMLInputElement;
                                switch (InputElement.type) {
                                    case "checkbox":
                                        InputElement.checked = data[name];
                                        break;
                                    default:
                                        InputElement.value = data[name];
                                        break;
                                }
                                break;
                            case "SELECT":
                                let SelectElement = SearchElement as HTMLInputElement;
                                SelectElement.value = data[name];
                                break;
                            case "TEXTAREA":
                                let TextAreaElement = SearchElement as HTMLTextAreaElement;
                                TextAreaElement.value = data[name];
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
        /**
         * 保存登录用户信息
         * @param data
         */
        public SaveLoginUserInfo(data): void {
            if (data != null) {
                let saveData: LoginUserModel = new LoginUserModel(data["ID"], data["Token"]);
                MLDMa.SetLocalData(this.ApplicationSettingM.SaveUserKey, saveData, true);
            }
            else {
                this.RemoveLoginUserInfo();
            }
        }
        /**
         * 移除登录用户信息
         */
        public RemoveLoginUserInfo(): void {
            MLDMa.RemoveLocalData(this.ApplicationSettingM.SaveUserKey);
        }
        /**
         * 获得登录用户信息
         */
        public GetLoginUserInfo(): LoginUserModel {
            return MLDMa.GetLocalData(this.ApplicationSettingM.SaveUserKey, true) as LoginUserModel;
        }
        /**
         * 显示密码按钮单击事件
         * @param e
         */
        public BtnShowPasswordEvent_Click(e: MouseEvent) {
            let btnElement = e.target as HTMLButtonElement;
            let targetID = btnElement.dataset.target;
            if (!MTMa.IsNullOrUndefinedOrEmpty(targetID)) {
                let inputElement = MDMa.$(targetID) as HTMLInputElement;
                if (!MTMa.IsNullOrUndefined(inputElement)) {
                    if (inputElement.type == "text") {
                        inputElement.type = "password";
                        btnElement.classList.remove("glyphicon-eye-close");
                        btnElement.classList.add("glyphicon-eye-open");
                    }
                    else {
                        inputElement.type = "text";
                        btnElement.classList.remove("glyphicon-eye-open");
                        btnElement.classList.add("glyphicon-eye-close");
                    }
                }
            }
        }
        /**
         * 输入框验证事件
         * @param e 事件对象
         * @param setting 错误提示设置对象
         */
        public InputInvalidEvent_Invalid(e: Event, setting: InvalidOptionsModel) {
            let options: InvalidOptionsModel = MTMa.IsNullOrUndefined(setting) ? new InvalidOptionsModel() : setting;
            let inputElement = e.target as HTMLInputElement;
            if (inputElement.validity.valueMissing) {
                common.SetInputErrorMessage(inputElement, options.Required);
            }
            else if (inputElement.validity.patternMismatch) {
                common.SetInputErrorMessage(inputElement, options.Pattern);
            }
            else if (inputElement.validity.rangeUnderflow) {
                common.SetInputErrorMessage(inputElement, options.Min);
            }
            else {
                common.SetInputErrorMessage(inputElement, options.Defult);
            }
        }
        /**
         * 设置错误信息
         * @param inputElement 输入元素
         * @param message 错误信息
         */
        public SetInputErrorMessage(inputElement: HTMLElement | string, message: string) {
            inputElement = MDMa.$(inputElement);
            let formGroupElement = inputElement.parentElement as HTMLElement;
            while (!MDMa.HasClass(formGroupElement, "form-group")) {
                formGroupElement = formGroupElement.parentElement;
            }
            formGroupElement.classList.add("has-error");
            let messageSpan = formGroupElement.lastElementChild;
            if (MDMa.HasClass(messageSpan, "help-block")) {
                messageSpan.textContent = message;
            }
        }
        /**
         * 清空错误信息
         */
        public ClearErrorMessage() {
            let ErrorMessages = document.getElementsByClassName("has-error");
            for (let i = ErrorMessages.length - 1; i >= 0; i--) {
                let messageSpan = ErrorMessages[i].lastElementChild;
                if (MDMa.HasClass(messageSpan, "help-block")) {
                    messageSpan.textContent = "";
                }
                ErrorMessages[i].classList.remove("has-error");
            }
        }
        /**
         * 跳转页面
         * @param act 标识
         */
        public GoToPage(act: string, params: string[] = null, isTop: boolean = false) {
            let url = this.ApplicationSettingM.DomainName;
            switch (act) {
                case "Login":
                    url += "Backstage/View/User/Login.html";
                    break;
                case "Index":
                    url += "Backstage/View/System/Index.html";
                    break;
                default:
                    url += "Backstage/View/User/Login.html";
                    break;
            }
            if (!MTMa.IsNullOrUndefined(params) && params.length > 0) {
                url += "?" + params.join("&");
            }
            window.location.href = url;
        }
        /**
         * 发送GetAjax请求
         * @param url 请求地址
         * @param data 请求参数
         * @param SFun 成功方法
         * @param SFun 失败方法
         * @param CFun 都执行方法
         * @param EFun 错误方法
         */
        public SendGetAjax(url: string, data: Object, SFun: Function, FFun: Function, CFun: Function, EFun: Function = null) {
            this.SendAjax(url, data, SFun, FFun, CFun, EFun, "get", "json");
        }
        /**
         * 发送PostAjax请求
         * @param url 请求地址
         * @param data 请求参数
         * @param SFun 成功方法
         * @param SFun 失败方法
         * @param CFun 都执行方法
         * @param EFun 错误方法
         * @param dataType 数据类型
         */
        public SendPostAjax(url: string, data: Object, SFun: Function, FFun: Function, CFun: Function, EFun: Function = null, dataType: string = "json") {
            let LoginUserM: LoginUserModel = this.GetLoginUserInfo();
            if (LoginUserM != null) {
                data["LoginUserID"] = LoginUserM.ID;
                data["LoginUserToken"] = LoginUserM.Token;
            }
            this.SendAjax(url, data, SFun, FFun, CFun, EFun, "post", dataType);
        }
        /**
         * 发送Ajax请求
         * @param url 请求地址
         * @param data 请求参数
         * @param SFun 成功方法
         * @param SFun 失败方法
         * @param CFun 都执行方法
         * @param EFun 错误方法
         * @param type 请求类型
         * @param dataType 数据类型
         */
        public SendAjax(url: string, data: Object, SFun: Function, FFun: Function, CFun: Function, EFun: Function = null, type: string = "post", dataType: string = "json") {
            url = this.ApplicationSettingM.DomainName + url;
            if (EFun == null) {
                EFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
                    switch (state) {
                        case 400: case 401:
                            common.RemoveLoginUserInfo();
                            common.GoToPage("Login", null, true);
                            break;
                        case 403:
                            common.ShowMessageBox("您的权限不足。");
                            break;
                        case 404:
                            common.ShowMessageBox("未能找到该资源。");
                            break;
                        default:
                            common.ShowMessageBox(resM["Message"] != null ? resM["Message"] : "应用程序出错了");
                            break;
                    }
                }
            }
            let SuccessFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
                if (resM["ResultType"] == 0) {
                    SFun(resM, xhr, state);
                }
                else if (resM["ResultType"] == 1) {
                    FFun(resM, xhr, state);
                }
                else {
                    EFun(resM, xhr, state);
                }
            }
            let config = new MateralTools.HttpConfigModel(url, type, data, dataType, SuccessFun, EFun, CFun);
            MateralTools.HttpManager.Send(config);
        }
        /**
         * 显示消息窗体
         * @param message 消息
         * @param title 标题
         */
        public ShowMessageBox(message: string, title: string = "提示"): void {
            let MessageBoxTitle = MDMa.$("MessageBoxTitle") as HTMLDivElement;
            let MessageBoxBody = MDMa.$("MessageBoxBody") as HTMLDivElement;
            if (!MTMa.IsNullOrUndefined(MessageBoxTitle)) {
                MessageBoxTitle.textContent = title;
            }
            if (!MTMa.IsNullOrUndefined(MessageBoxBody)) {
                MessageBoxBody.textContent = message;
            }
            $('#MessageBox').modal('toggle');
        }
        /**
         * 添加信息提示窗体
         */
        private AddMessageBoxArticle() {
            let element = MDMa.$("MessageBox") as HTMLElement;
            if (MTMa.IsNullOrUndefined(element)) {
                element = document.createElement("article") as HTMLElement;
                element.id = "MessageBox";
                document.body.appendChild(element);
            }
            else {
                element.innerHTML = "";
            }
            MDMa.AddClass(element, "modal fade");
            element.setAttribute("tabindex", "-1");
            element.setAttribute("role", "dialog");
            element.setAttribute("aria-labelledby", "MessageBoxLabel");
            let modalDialog = document.createElement("div");
            MDMa.AddClass(modalDialog, "modal-dialog");
            modalDialog.setAttribute("role", "document");
            element.appendChild(modalDialog);
            let modalContent = document.createElement("div");
            MDMa.AddClass(modalContent, "modal-content");
            modalDialog.appendChild(modalContent);
            let modalHeader = document.createElement("div");
            MDMa.AddClass(modalHeader, "modal-header");
            modalContent.appendChild(modalHeader);
            let btnClose = document.createElement("button");
            MDMa.AddClass(btnClose, "close");
            btnClose.type = "button";
            btnClose.dataset.dismiss = "modal";
            btnClose.setAttribute("aria-label", "Close");
            let btnCloseSpan = document.createElement("span");
            btnCloseSpan.setAttribute("aria-hidden", "true");
            btnCloseSpan.innerHTML = "&times;";
            btnClose.appendChild(btnCloseSpan);
            modalHeader.appendChild(btnClose);
            let modalTitle = document.createElement("h4");
            MDMa.AddClass(modalTitle, "modal-title");
            modalTitle.id = "MessageBoxTitle";
            modalTitle.innerHTML = "提示";
            modalHeader.appendChild(modalTitle);
            let modalBody = document.createElement("div");
            MDMa.AddClass(modalBody, "modal-body");
            modalBody.id = "MessageBoxBody";
            modalContent.appendChild(modalBody);
            let modalFooter = document.createElement("div");
            MDMa.AddClass(modalFooter, "modal-footer");
            modalContent.appendChild(modalFooter);
            let btnOK = document.createElement("button");
            btnOK.id = "MessageBoxBtnOK";
            btnOK.type = "button";
            MDMa.AddClass(btnOK, "btn btn-default");
            btnOK.dataset.dismiss = "modal";
            btnOK.innerHTML = "确定";
            modalFooter.appendChild(btnOK);
        }
        /**
         * 回车关闭提示窗体
         * @param e
         */
        private KeyEnterCloseMessageBoxEvent(e: KeyboardEvent) {
            if (e.keyCode == 13) {
                let MessageBox = MDMa.$("MessageBox") as HTMLDivElement;
                if (MDMa.HasClass(MessageBox, "modal fade in")) {
                    let MessageBoxBtnOK = document.getElementById("MessageBoxBtnOK") as HTMLButtonElement;
                    if (!MTMa.IsNullOrUndefined(MessageBoxBtnOK)) {
                        MessageBoxBtnOK.click();
                    }
                }
            }
        }
        /**
         * 检测登录
         * @param gotoLogin 没登录是否跳转登录
         */
        public IsLogin(gotoLogin: boolean = false) {
            let userInfo = common.GetLoginUserInfo();
            if (MTMa.IsNullOrUndefined(userInfo)) {
                if (gotoLogin) {
                    common.GoToPage("Login", ["from=" + encodeURIComponent(window.location.pathname + window.location.search)], true);
                    return false;
                }
                else {
                    return false;
                }
            }
            else {
                return true;
            }
        }
        /**
         * 设置标题
         * @param headerTitle 头部标题
         * @param pageTitle 页面标题
         */
        public SetTitle(headerTitle: string, pageTitle: string = null) {
            if (MTMa.IsNullOrUndefinedOrEmpty(pageTitle)) {
                pageTitle = headerTitle;
            }
            let HeaderTitle = MDMa.$("HeaderTitle") as HTMLElement;
            if (!MTMa.IsNullOrUndefined(HeaderTitle)) {
                HeaderTitle.textContent = headerTitle;
            }
            let PageTitle = MDMa.$("PageTitle") as HTMLElement;
            if (!MTMa.IsNullOrUndefined(PageTitle)) {
                PageTitle.textContent = pageTitle;
            }
        }
        /**
         * 获取滚动条位置
         */
        public GetScrollTop() {
            var scrollTop = 0;
            if (document.documentElement && document.documentElement.scrollTop) {
                scrollTop = document.documentElement.scrollTop;
            }
            else if (document.body) {
                scrollTop = document.body.scrollTop;
            }
            return scrollTop;
        }
        /**
         * 获取可见高度
         */
        public GetClientHeight() {
            var clientHeight = 0;
            if (document.body.clientHeight && document.documentElement.clientHeight) {
                var clientHeight = (document.body.clientHeight < document.documentElement.clientHeight) ? document.body.clientHeight : document.documentElement.clientHeight;
            }
            else {
                var clientHeight = (document.body.clientHeight > document.documentElement.clientHeight) ? document.body.clientHeight : document.documentElement.clientHeight;
            }
            return clientHeight;
        }
        /**
         * 设置验证码图片
         * @param img 验证码图片元素
         */
        public LoadValidateImg(img: HTMLImageElement) {
            let dt = MateralTools.ToolManager.DateTimeFormat(new Date(), "yyyyMMddHHmmss");
            img.setAttribute("src", this.ApplicationSettingM.DomainName + "api/ValidateCode/GetValidateCode?time=" + dt);
        }
        /**
         * 清空模拟框form
         * @param id
         */
        public ClearModalForm(id: string | HTMLDivElement) {
            this.ClearErrorMessage();
            let EditModal = MDMa.$(id);
            if (!MTMa.IsNullOrUndefined(EditModal)) {
                let elements: any = EditModal.getElementsByTagName("input");
                for (var i = 0; i < elements.length; i++) {
                    switch (elements[i].type) {
                        case "checkbox":
                            elements[i].checked = false;
                            break;
                        case "radio":
                            elements[i].checked = false;
                            break;
                        default:
                            elements[i].value = "";
                            break;
                    }
                }
                elements = EditModal.getElementsByTagName("select");
                for (var i = 0; i < elements.length; i++) {
                    elements[i].selectedIndex = 0;
                }
                elements = EditModal.getElementsByTagName("textarea");
                for (var i = 0; i < elements.length; i++) {
                    elements[i].value = "";
                }
            }
        }
    }
    /**
     * 应用程序设置模型
     */
    class ApplicationSettingModel {
        /*调试模型*/
        public static IsDebug: boolean = false;
        /*域名*/
        private _domainName: string = "http://www.lwmcdz.com/";
        /*调试域名*/
        private _deBugDomainName: string = "http://localhost:63699/";
        /*保存用户的值*/
        public SaveUserKey: string = "LOGINUSERKEY";
        /*服务器域名*/
        public get DomainName() {
            if (!ApplicationSettingModel.IsDebug) {
                return this._domainName;
            }
            else {
                return this._deBugDomainName;
            }
        }
    }
    /**
     * 验证模型
     */
    export class InvalidOptionsModel {
        /*必填*/
        public Required: string = "输入不能为空";
        /*正则*/
        public Pattern: string = "输入未能通过验证";
        /*过小*/
        public Min: string = "输入过小";
        /*过大*/
        public Max: string = "输入过大";
        /*默认*/
        public Defult: string = "输入有误";
    }
    /**
     * 登录用户模型
     */
    export class LoginUserModel {
        /*用户ID*/
        public ID: string;
        /*用户Token*/
        public Token: string;
        /**
         * 构造函数
         * @param id 用户唯一标识
         * @param token 用户Token
         */
        constructor(id: string, token: string) {
            this.ID = id;
            this.Token = token;
        }
    }
    /**
     * 登录用户模型
     */
    export class MPagingModel {
        /*总数据*/
        public DataCount: number;
        /*总页数*/
        public PagingCount: number;
        /*当前页数*/
        public PagingIndex: number;
        /*每页显示数量*/
        public PagingSize: number;
    }
    /**
     * 公共处理对象
     */
    export const common: Common = new Common();
}