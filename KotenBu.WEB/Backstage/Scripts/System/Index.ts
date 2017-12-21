/// <reference path="../../../scripts/jquery.d.ts" />
/// <reference path="../../../scripts/base.ts" />
/// <reference path="../../../lib/m-tools/m-tools.ts" />
namespace KotenBu.Backstage {
    /**
     * 首页
     */
    class IndexPage {
        private static PageData = {
            LoginUserID: "",
            Permissions:[]
        };
        /**
         * 构造方法
         */
        constructor() {
            if (common.IsLogin(true)) {
                this.BindAllSex();
                this.GetMenuInfoByLoginUser();
                this.BindEvent();
            }
        }
        /**
         * 绑定事件
         */
        private BindEvent() {
            MDMa.AddEvent("BtnSave", "click", this.BtnSaveEvent_Click);
            MDMa.AddEvent("BtnSavePassword", "click", this.BtnSavePasswordEvent_Click);
            MDMa.AddEvent("BtnLogOut", "click", this.BtnLogOutEvent_Click);
            MDMa.AddEvent("InputNickName", "invalid", function (e: Event) {
                let setting: InvalidOptionsModel = new InvalidOptionsModel();
                setting.Required = "不能为空";
                common.InputInvalidEvent_Invalid(e, setting);
            });
            MDMa.AddEvent("InputUserName", "invalid", function (e: Event) {
                let element = e.target as HTMLInputElement;
                let setting: InvalidOptionsModel = new InvalidOptionsModel();
                setting.Max = "长度不能超过" + element.maxLength;
                setting.Required = "不能为空";
                setting.Pattern = "只可使用英文和数字";
                common.InputInvalidEvent_Invalid(e, setting);
            });
            MDMa.AddEvent("InputMobile", "invalid", function (e: Event) {
                let element = e.target as HTMLInputElement;
                let setting: InvalidOptionsModel = new InvalidOptionsModel();
                setting.Max = "长度不能超过" + element.maxLength;
                setting.Required = "不能为空";
                setting.Pattern = "格式错误";
                common.InputInvalidEvent_Invalid(e, setting);
            });
            MDMa.AddEvent("InputEmail", "invalid", function (e: Event) {
                let element = e.target as HTMLInputElement;
                let setting: InvalidOptionsModel = new InvalidOptionsModel();
                setting.Max = "长度不能超过" + element.maxLength;
                setting.Required = "不能为空";
                setting.Pattern = "邮箱格式不正确";
                common.InputInvalidEvent_Invalid(e, setting);
            });
            MDMa.AddEvent("InputTrueName", "invalid", function (e: Event) {
                let element = e.target as HTMLInputElement;
                let setting: InvalidOptionsModel = new InvalidOptionsModel();
                setting.Max = "长度不能超过" + element.maxLength;
                setting.Required = "不能为空";
                setting.Pattern = "请填写正确的中文真实姓名";
                common.InputInvalidEvent_Invalid(e, setting);
            });
            MDMa.AddEvent("InputOldPassword", "invalid", function (e: Event) {
                let setting: InvalidOptionsModel = new InvalidOptionsModel();
                setting.Required = "不能为空";
                common.InputInvalidEvent_Invalid(e, setting);
            });
            MDMa.AddEvent("InputNewPassword1", "invalid", function (e: Event) {
                let setting: InvalidOptionsModel = new InvalidOptionsModel();
                setting.Required = "不能为空";
                common.InputInvalidEvent_Invalid(e, setting);
            });
            MDMa.AddEvent("InputNewPassword2", "invalid", function (e: Event) {
                let setting: InvalidOptionsModel = new InvalidOptionsModel();
                setting.Required = "不能为空";
                common.InputInvalidEvent_Invalid(e, setting);
            });
        }
        /**
         * 获得所有性别枚举信息
         */
        private BindAllSex() {
            let url: string = "api/User/GetAllSex";
            let data = null;
            let SFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
                let sexEnumMs = resM["Data"] as Array<any>;
                let InputSex = MDMa.$("InputSex") as HTMLSelectElement;
                InputSex.innerHTML = "";
                for (var i = 0; i < sexEnumMs.length; i++) {
                    let option = new Option(sexEnumMs[i]["EnumName"], sexEnumMs[i]["EnumValue"]);
                    InputSex.options.add(option);
                }
                IndexPage.BindLoginUserInfo();
            };
            let FFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
                common.ShowMessageBox(resM["Message"])
            };
            let CFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
            };
            common.SendGetAjax(url, data, SFun, FFun, CFun);
        }
        /**
         * 绑定登录用户信息
         */
        private static BindLoginUserInfo() {
            let loginUserInfo = common.GetLoginUserInfo();
            let url: string = "api/User/GetUserViewInfoByID";
            let data = {
                ID: loginUserInfo.ID
            };
            let SFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
                let UserInfo = resM["Data"];
                common.BindInputInfo(UserInfo);
                let ShowName = MTMa.IsNullOrUndefinedOrEmpty(UserInfo["TrueName"]) ? UserInfo["NickName"] : UserInfo["TrueName"];
                let LoginUserName = MDMa.$("LoginUserName") as HTMLDivElement;
                LoginUserName.insertBefore(document.createTextNode(ShowName), LoginUserName.childNodes[0]);
            };
            let FFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
                common.ShowMessageBox(resM["Message"])
            };
            let CFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
            };
            common.SendGetAjax(url, data, SFun, FFun, CFun);
        }
        /**
         * 保存按钮单击事件
         * @param e
         */
        private BtnSaveEvent_Click(e: MouseEvent) {
            common.ClearErrorMessage();
            let data = IndexInputModel.GetInputData();
            if (data != null) {
                let BtnElement = e.target as HTMLButtonElement;
                BtnElement.textContent = "保存中......";
                BtnElement.disabled = true;
                let url: string = "api/User/UpdateMyInfo";
                let SFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
                    $('#EditModal').modal('toggle');
                };
                let FFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
                };
                let CFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
                    BtnElement.textContent = "保存";
                    BtnElement.disabled = false;
                    common.ShowMessageBox(resM["Message"]);
                };
                common.SendPostAjax(url, data, SFun, FFun, CFun);
            }
        }
        /**
         * 退出登录按钮单击事件
         * @param e
         */
        private BtnLogOutEvent_Click(e: MouseEvent) {
            common.RemoveLoginUserInfo();
            common.GoToPage("Login");
        }
        /**
         * 保存密码按钮单击事件
         * @param e
         */
        private BtnSavePasswordEvent_Click(e: MouseEvent) {
            common.ClearErrorMessage();
            let data = EditPasswordModel.GetInputData();
            if (data != null) {
                let BtnElement = e.target as HTMLButtonElement;
                BtnElement.textContent = "保存中......";
                BtnElement.disabled = true;
                let url: string = "api/User/EditMyPassword";
                let SFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
                    $('#EditPassowrdModal').modal('toggle');
                };
                let FFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
                };
                let CFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
                    BtnElement.textContent = "保存";
                    BtnElement.disabled = false;
                    common.ShowMessageBox(resM["Message"]);
                };
                common.SendPostAjax(url, data, SFun, FFun, CFun);
            }
        }
        /**
         * 根据登录获得菜单信息
         */
        private GetMenuInfoByLoginUser() {
            let url: string = "api/User/GetMenuInfoByLoginUser";
            let data = {};
            let SFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
                let TopNav = MDMa.$("TopNav");
                TopNav.innerHTML = "";
                IndexPage.PageData.Permissions = resM["Data"]["Items"];
                for (let i = 0; i < IndexPage.PageData.Permissions.length; i++) {
                    let li = document.createElement("li");
                    let a = document.createElement("a");
                    if (IndexPage.PageData.Permissions[i]["Ico"] != null) {
                        let ico = document.createElement("i");
                        MDMa.AddClass(ico, IndexPage.PageData.Permissions[i]["Ico"]);
                        a.appendChild(ico);
                    }
                    a.appendChild(document.createTextNode(IndexPage.PageData.Permissions[i].Name));
                    a.dataset.topid = "TopNav";
                    if (IndexPage.PageData.Permissions[i].Items == null) {
                        a.dataset.href = IndexPage.PageData.Permissions[i]["Code"];
                        MDMa.AddEvent(a, "click", function (e) {
                            IndexPage.OpenIFramePage(e);
                            MDMa.AddClass("MainPanel", "NoLeft");
                        });
                    }
                    else {
                        a.dataset.index = i.toString();
                        MDMa.AddEvent(a, "click", IndexPage.BindLeftMenu);
                    }
                    li.appendChild(a);
                    TopNav.appendChild(li);
                }
            };
            let FFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
                common.ShowMessageBox(resM["Message"])
            };
            let CFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
            };
            common.SendPostAjax(url, data, SFun, FFun, CFun);
        }
        /**
         * 打开左部页面
         * @param e
         */
        public static OpenIFramePage(e: MouseEvent) {
            let btn = e.target as HTMLElement;
            while (!btn.dataset.href) {
                btn = btn.parentElement as HTMLAnchorElement;
            }
            let href = btn.dataset.href;
            let MainFrame = document.getElementById("MainFrame") as HTMLIFrameElement;
            MainFrame.setAttribute("src", href);
            IndexPage.SetActive(btn);
        }
        /**
         * 设置选择样式
         * @param ActiveElement 要选择的元素
         */
        public static SetActive(ActiveElement: HTMLElement) {
            let TopElement = MDMa.$(ActiveElement.dataset.topid);
            if (TopElement) {
                let activeElement = TopElement.getElementsByClassName("active");
                for (var i = activeElement.length - 1; i == activeElement.length || i >= 0; i--) {
                    MDMa.RemoveClass(activeElement[i], "active");
                }
            }
            MDMa.AddClass(ActiveElement.parentElement, "active");
            while (MDMa.HasClass(ActiveElement.parentElement.parentElement, "collapse")) {
                MDMa.AddClass(ActiveElement.parentElement.parentElement.parentElement, "active");
                ActiveElement = ActiveElement.parentElement.parentElement.parentElement;
            }
        }
        /**
         * 绑定左部菜单
         * @param e
         */
        public static BindLeftMenu(e: MouseEvent) {
            let btn = e.target as HTMLElement;
            while (!btn.dataset.index) {
                btn = btn.parentElement as HTMLAnchorElement;
            }
            let index = btn.dataset.index;
            MDMa.RemoveClass("MainPanel", "NoLeft");
            let rightM = IndexPage.PageData.Permissions[index]["Items"];
            let mainNav = MDMa.$("main-nav");
            mainNav.innerHTML = "";
            for (let i = 0; i < rightM["length"]; i++) {
                let li = IndexPage.GetLeftHrefBtn(rightM[i]);
                mainNav.appendChild(li);
            }
            //let MainFrame = document.getElementById("MainFrame") as HTMLIFrameElement;
            //MainFrame.setAttribute("src", '/Home/Helper');
            IndexPage.SetActive(btn);
        }
        /**
         * 获得左部菜单单项
         * @param model
         */
        public static GetLeftHrefBtn(model: any): HTMLLIElement {
            let li = document.createElement("li");
            let a = document.createElement("a");
            if (model["Ico"] != null) {
                let ico = document.createElement("i");
                MDMa.AddClass(ico, model["Ico"]);
                a.appendChild(ico);
            }
            a.appendChild(document.createTextNode(model["Name"]));
            if (model["Items"] == null) {
                a.dataset.href = model["Code"];
                a.dataset.topid = "main-nav";
                MDMa.AddEvent(a, "click", function (e) {
                    IndexPage.OpenIFramePage(e);
                });
                li.appendChild(a);
            }
            else {
                let span = document.createElement("span");
                MDMa.AddClass(span, "pull-right glyphicon glyphicon-chevron-down");
                a.setAttribute("href", "#" + model["Code"]);
                MDMa.AddClass(a, "nav-header collapsed");
                a.dataset.toggle = "collapse";
                let subUl = document.createElement("ul");
                subUl.setAttribute("id", model["Code"]);
                MDMa.AddClass(subUl, "nav nav-list collapse secondmenu");
                for (var i = 0; i < model["Items"]["length"]; i++) {
                    let subli = IndexPage.GetLeftHrefBtn(model["Items"][i]);
                    subUl.appendChild(subli);
                }
                a.appendChild(span);
                li.appendChild(a);
                li.appendChild(subUl);
            }
            return li;
        }
    }
    /**
     * 登录输入模型
     */
    class IndexInputModel {
        /*唯一标识*/
        public ID: string;
        /*昵称*/
        public NickName: string;
        /*用户名*/
        public UserName: string;
        /*手机号码*/
        public Mobile: string;
        /*邮件*/
        public Email: string;
        /*真实姓名*/
        public TrueName: string;
        /*性别*/
        public Sex: string;
        /**
         * 获得输入模型
         */
        public static GetInputData(): IndexInputModel {
            let data: IndexInputModel = null;
            let InputForm = document.forms["InputForm"] as HTMLFormElement;
            if (!MTMa.IsNullOrUndefined(InputForm) && InputForm.checkValidity()) {
                let loginUserInfo = common.GetLoginUserInfo();
                data = {
                    ID: loginUserInfo.ID,
                    NickName: MDMa.GetInputValue("InputNickName"),
                    UserName: MDMa.GetInputValue("InputUserName"),
                    Mobile: MDMa.GetInputValue("InputMobile"),
                    Email: MDMa.GetInputValue("InputEmail"),
                    TrueName: MDMa.GetInputValue("InputTrueName"),
                    Sex: MDMa.GetInputValue("InputSex"),
                };
            }
            return data;
        }
    }
    /**
     * 修改密码模型
     */
    class EditPasswordModel {
        /*唯一标识*/
        public ID: string;
        /*旧密码*/
        public OldPassword: string;
        /*新密码*/
        public NewPassword: string;
        /**
         * 获得输入模型
         */
        public static GetInputData(): EditPasswordModel {
            let data: EditPasswordModel = null;
            let InputForm = document.forms["PasswordInputForm"] as HTMLFormElement;
            if (!MTMa.IsNullOrUndefined(InputForm) && InputForm.checkValidity()) {
                let loginUserInfo = common.GetLoginUserInfo();
                data = {
                    ID: loginUserInfo.ID,
                    OldPassword: MDMa.GetInputValue("InputOldPassword"),
                    NewPassword: MDMa.GetInputValue("InputNewPassword1"),
                };
                let NewPassword = MDMa.GetInputValue("InputNewPassword2");
                if (data.NewPassword != NewPassword) {
                    data = null;
                    common.SetInputErrorMessage("InputNewPassword1", "两次输入的密码不一样");
                    common.SetInputErrorMessage("InputNewPassword2", "两次输入的密码不一样");
                }
            }
            return data;
        }
    }
    /*页面加载完成时触发*/
    MDMa.AddEvent(window, "load", function () {
        let pageM = new IndexPage();
    });
}