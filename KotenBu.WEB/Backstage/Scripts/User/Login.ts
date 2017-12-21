/// <reference path="../../../scripts/jquery.d.ts" />
/// <reference path="../../../scripts/base.ts" />
/// <reference path="../../../lib/m-tools/m-tools.ts" />
namespace KotenBu.Backstage {
    /**
     * 登录页面
     */
    class LoginPage {
        /**
         * 构造方法
         */
        constructor() {
            this.BindEvent();
            let InputUserName = MDMa.$("InputUserName") as HTMLInputElement;
            let InputPassword = MDMa.$("InputPassword") as HTMLInputElement;
            let InputValidateCode = MDMa.$("InputValidateCode") as HTMLInputElement;
            InputUserName.focus();
        }
        /**
         * 绑定事件
         */
        private BindEvent(): void {
            MDMa.AddEvent("ValidateImg", "click", this.ValidateImgEvent_Click);
            MDMa.AddEvent("BtnLogin", "click", this.BtnLoginEvent_Click);
            MDMa.AddEvent("InputUserName", "keypress", this.InputEvent_KeyPress);
            MDMa.AddEvent("InputPassword", "keypress", this.InputEvent_KeyPress);
            MDMa.AddEvent("InputValidateCode", "keypress", this.InputEvent_KeyPress);
            MDMa.AddEvent("InputUserName", "invalid", function (e: Event) {
                let setting: InvalidOptionsModel = new InvalidOptionsModel();
                setting.Required = "用户名不能为空";
                common.InputInvalidEvent_Invalid(e, setting);
            });
            MDMa.AddEvent("InputPassword", "invalid", function (e: Event) {
                let setting: InvalidOptionsModel = new InvalidOptionsModel();
                setting.Required = "密码不能为空";
                common.InputInvalidEvent_Invalid(e, setting);
            });
            MDMa.AddEvent("InputValidateCode", "invalid", function (e: Event) {
                let setting: InvalidOptionsModel = new InvalidOptionsModel();
                setting.Required = "验证码不能为空";
                common.InputInvalidEvent_Invalid(e, setting);
            });
        }
        /**
         * 重新加载验证码
         */
        private ValidateImgEvent_Click(e: MouseEvent): void {
            let img = MDMa.$("ValidateImg") as HTMLImageElement;
            common.LoadValidateImg(img);
        }
        /**
         * 输入框回车事件
         * @param e
         */
        private InputEvent_KeyPress(e: KeyboardEvent) {
            if (e.keyCode == 13) {
                let BtnElement = MDMa.$("BtnLogin") as HTMLButtonElement;
                LoginPage.Login(BtnElement);
            }
        }
        /**
         * 登录按钮单击事件
         * @param e
         */
        private BtnLoginEvent_Click(e: MouseEvent): void {
            let BtnElement = e.target as HTMLButtonElement;
            LoginPage.Login(BtnElement);
        }
        /**
         * 登录方法
         */
        private static Login(BtnElement: HTMLButtonElement) {
            common.ClearErrorMessage();
            let data = LoginInputModel.GetInputData();
            if (data != null) {
                BtnElement.textContent = "登录中......";
                BtnElement.disabled = true;
                let url: string = "api/User/AdminLogin";
                let SFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
                    common.SaveLoginUserInfo(resM["Data"]);
                    common.GoToPage("Index");
                };
                let FFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
                    common.ShowMessageBox(resM["Message"]);
                };
                let CFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
                    BtnElement.textContent = "登录";
                    BtnElement.disabled = false;
                };
                common.SendPostAjax(url, data, SFun, FFun, CFun);
            }
        }
    }
    /**
     * 登录输入模型
     */
    class LoginInputModel {
        /*用户名*/
        public UserName: string;
        /*密码*/
        public Password: string;
        /*验证码*/
        public ValidateCode: string;
        /**
         * 获得输入模型
         */
        public static GetInputData(): LoginInputModel {
            let data: LoginInputModel = null;
            let InputForm = document.forms["InputForm"] as HTMLFormElement;
            if (!MTMa.IsNullOrUndefined(InputForm) && InputForm.checkValidity()) {
                data = {
                    UserName: MDMa.GetInputValue("InputUserName"),
                    Password: MDMa.GetInputValue("InputPassword"),
                    ValidateCode: MDMa.GetInputValue("InputValidateCode"),
                };
            }
            return data;
        }
    }
    /*页面加载完成时触发*/
    MDMa.AddEvent(window, "load", function () {
        let pageM = new LoginPage();
    });
}