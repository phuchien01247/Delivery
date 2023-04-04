<script>
import { required } from "vuelidate/lib/validators";
import appConfig from "@/app.config";
import Vue from "vue";

import {VueRecaptcha} from "vue-recaptcha";
import LetterCube from "@/components/LetterCube";

/**
 * Login component
 */
export default {
  page: {
    title: "Đăng nhập",
    meta: [{ name: "description", content: appConfig.description }],
  },
  // eslint-disable-next-line vue/no-unused-components
  components: {VueRecaptcha, LetterCube},
  validations: {
    model:{
      userName: {
        required,
      },
      password: {
        required
      }
    }
  },
  data() {
    return {
      capcha: null,
      email: "admin@themesbrand.com",
      password: "123456",
      submitted: false,
      authError: null,
      tryingToLogIn: false,
      isAuthError: false,
      modelAuth:{
        isAuthError: false,
        message: null
      },
      model:{
        userName: "",
        password: ""
      },
      vanThuTruong: "D2570530.000453"
    };
  },
  computed: {
    isVanThu(){
      return this.vanThuTruong == this.model.userName;
    }
  },
  methods: {
    submit(response){
      this.capcha = response;
    },
    async Login(e) {
      e.preventDefault();
      if(!this.capcha && process.env.VUE_APP_ENV != 'development' && !this.isVanThu) {
        this.modelAuth.isAuthError = true;
        this.modelAuth.message = "Xác nhận đã nhập mã captcha";
        return;
      }
      this.submitted = true;
      this.$v.$touch();
      if (this.$v.$invalid) {
        return;
      } else {
        let loader = this.$loading.show({
          container: this.$refs.formContainer,
        },{
          default: this.$createElement(LetterCube),
        });
        // await setTimeout(function () {
        //  }, 5000);
        await this.$store.dispatch("authStore/login", this.model).then(async (res) => {
          if (res.resultCode === 'SUCCESS') {
            await new Promise(resolve => setTimeout(resolve, 1000));
            localStorage.setItem('auth-user', JSON.stringify(res.data));
            localStorage.setItem("user-token", JSON.stringify(res.data.token));

            if (res.data.user) {
              if (res.data.user.menuItems) {
                localStorage.setItem("menuItems", JSON.stringify(res.data.user.menuItems));
              }
            }
            Vue.prototype.$auth_token = res.data.token;
            this.showModal = false;
            this.model = {};
            this.modelAuth.isAuthError = false;
            window.location.href = '/'
          } else {
            if (res.code == 400) {
              this.modelAuth.isAuthError = true;
              this.modelAuth.message = "Lỗi! Hãy kiểm tra kết nối mạng!";
            } else {
              this.modelAuth.isAuthError = true;
              this.modelAuth.message = res.resultString;
            }
            loader.hide();
          }

        })
            .finally(() => {
              loader.hide();
            });
      }
      this.submitted = false;
    },
  }
};
</script>

<template>
  <div id="app">
    <div>
      <div class="accountbg"></div>
      <div class="wrapper-page account-page-full">
        <div class="card shadow-none">
          <div class="card-block">
            <div class="account-box">
              <div class="card-box shadow-none p-4">
                <div class="p-2">
                  <div class="text-center mt-4">
                    <a href="/" class="router-link-active">
                      <img src="@/assets/long_logo.png" height="80" alt="logo">
                    </a>
                    </div>
                    <h4 class="font-size-18 mt-5 text-center">Chào mừng !</h4>
                    <p class="text-muted text-center">Đến với hệ thống hỗ trợ người dùng.</p>
                        <div class="mt-4">
                          <b-alert
                              v-model="modelAuth.isAuthError"
                              variant="danger"
                              class="mt-4"
                              dismissible
                          >
                            {{ modelAuth.message }}
                          </b-alert>
                          <b-form
                              @submit.prevent="Login"  ref="formContainer"
                              class="form-horizontal mt-4"
                          >
                          
                            <b-form-group
                                id="input-group-1"
                                label="Tài khoản"
                                label-for="input-1"
                                class="mb-3 text-white"
                                label-class="form-label"
                            >
                              <b-form-input
                                  id="input-1"
                                  v-model="model.userName"
                                  type="text"
                                  placeholder="Nhập tài khoản"
                                  :class="{ 'is-invalid': submitted && $v.model.userName.$error }"
                                  class="cs-form-input-login"
                              ></b-form-input>
                              <div
                                  v-if="submitted && $v.model.userName.$error"
                                  class="invalid-feedback"
                              >
                                <span v-if="!$v.model.userName.required">Tài khoản không được trống.</span>
                              </div>
                            </b-form-group>

                            <b-form-group
                                id="input-group-2"
                                label="Mật khẩu"
                                label-for="input-2"
                                class="mb-3 text-white"
                                label-class="form-label"
                            >
                              <b-form-input
                                  id="input-2"
                                  v-model="model.password"
                                  type="password"
                                  placeholder="Nhập mật khẩu"
                                  :class="{ 'is-invalid': submitted && $v.model.password.$error }"
                                  class="cs-form-input-login"
                              ></b-form-input>
                              <div
                                  v-if="submitted && !$v.model.password.required"
                                  class="invalid-feedback"
                              >
                                Mật khẩu không được trống.
                              </div>
                            </b-form-group>

                            <div class="form-group row">
                              <!--              <div class="col-sm-6">-->
                              <!--                <div class="form-check">-->
                              <!--                  <input-->
                              <!--                      type="checkbox"-->
                              <!--                      class="form-check-input"-->
                              <!--                      id="customControlInline"-->
                              <!--                  />-->
                              <!--                  <label-->
                              <!--                      class="form-check-label text-white"-->
                              <!--                      for="customControlInline"-->
                              <!--                  >Ghi nhớ tài khoản</label-->
                              <!--                  >-->
                              <!--                </div>-->
                              <!--              </div>-->
                              <div class="my-2 d-flex justify-content-center align-items-center capcha-box">
                                <vue-recaptcha v-if="!isVanThu"  @verify="submit" sitekey="6LeBwrgjAAAAAA4k6n8a21lzx_VqcfWR78yVnHhA"></vue-recaptcha>
                              </div>
                              <div class="col-sm-12 text-center">
                                <b-button type="submit" class="btn-submit-login text-uppercase fw-bold"
                                > Đăng nhập</b-button
                                >
                              </div>
                            </div>

                            <!--                  <div class="mt-2 mb-0 row">-->
                            <!--                    <div class="col-12 mt-4">-->
                            <!--                      <router-link to="/forgot-password">-->
                            <!--                        <i class="mdi mdi-lock"></i> Forgot your password?-->
                            <!--                      </router-link>-->
                            <!--                    </div>-->
                            <!--                  </div>-->
                          </b-form>
                        </div>
                  
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style lang="scss" >
.accountbg{
  background-image: url("~@/assets/images/bg/new_bg.jpg");
  background-repeat: no-repeat;
  background-size: cover;
  height: 100vh;
  overflow-x: hidden;
  &.wrapper-page account-page-full{
    height: 100% !important;
  }
  &.card-block{
      background: transparent;
      margin: auto;
      padding: 20px;
      border-radius: 10px;
      box-shadow: rgba(0, 0, 0, 0.25) 0px 54px 55px, rgba(0, 0, 0, 0.12) 0px -12px 30px, rgba(0, 0, 0, 0.12) 0px 4px 6px, rgba(0, 0, 0, 0.17) 0px 12px 13px, rgba(0, 0, 0, 0.09) 0px -3px 5px;
  
    }
}

</style>
