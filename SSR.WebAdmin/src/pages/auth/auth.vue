<script>
import { required } from "vuelidate/lib/validators";
import appConfig from "@/app.config";
import Vue from "vue";

import { VueRecaptcha } from "vue-recaptcha";
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
  components: { VueRecaptcha, LetterCube },
  validations: {
    model: {
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
      modelAuth: {
        isAuthError: false,
        message: null
      },
      model: {
        userName: "",
        password: ""
      },
      vanThuTruong: "D2570530.000453"
    };
  },
  computed: {
    isVanThu() {
      return this.vanThuTruong == this.model.userName;
    }
  },
  methods: {
    submit(response) {
      this.capcha = response;
    },
    async Login(e) {
      e.preventDefault();
      if (!this.capcha && process.env.VUE_APP_ENV != 'development' && !this.isVanThu) {
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
        }, {
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
    <div class="account-pages">
        <div class="row justify-content-left align-items-left login-box">
          <div class="col-md-6 col-lg-6 col-xl-4 bg-login-box d-flex justify-content-center align-items-center">
            <div>

              <div class="text-primary text-center">
                <a href="/" class="logo-login mb-1">
                  <img src="@/assets/images/logo_new.png" alt="logo" class="d-inline-block" />
                  <h2 class="text-center" style="color:black">CHÀO MỪNG !</h2>
                    <p class="font-size-18 text-center" style="color:black">Đến với hệ thống hỗ trợ yêu cầu người dùng.</p>
                </a>
              </div>

              <div class="px-3">
                <b-alert v-model="modelAuth.isAuthError" variant="danger" class="mt-4" dismissible>
                  {{ modelAuth.message }}
                </b-alert>
                <b-form @submit.prevent="Login" ref="formContainer" class="form-horizontal mt-4">
                  <b-form-group id="input-group-1" label="Tài khoản" label-for="input-1" class="mb-3 text-white"
                    label-class="form-label">
                    <b-form-input id="input-1" v-model="model.userName" type="text" placeholder="Nhập tài khoản"
                      :class="{ 'is-invalid': submitted && $v.model.userName.$error }"
                      class="cs-form-input-login"></b-form-input>
                    <div v-if="submitted && $v.model.userName.$error" class="invalid-feedback">
                      <span v-if="!$v.model.userName.required">Tài khoản không được trống.</span>
                    </div>
                  </b-form-group>

                  <b-form-group id="input-group-2" label="Mật khẩu" label-for="input-2" class="mb-3 text-white"
                    label-class="form-label">
                    <b-form-input id="input-2" v-model="model.password" type="password" placeholder="Nhập mật khẩu"
                      :class="{ 'is-invalid': submitted && $v.model.password.$error }"
                      class="cs-form-input-login"></b-form-input>
                    <div v-if="submitted && !$v.model.password.required" class="invalid-feedback">
                      Mật khẩu không được trống.
                    </div>
                  </b-form-group>

                  <div class="form-group row">

                    <div class="my-2 d-flex justify-content-center align-items-center capcha-box">
                      <vue-recaptcha v-if="!isVanThu" @verify="submit"
                        sitekey="6LeBwrgjAAAAAA4k6n8a21lzx_VqcfWR78yVnHhA"></vue-recaptcha>
                    </div>
                    <div class="col-sm-12 text-center">
                          <b-button type="submit" class="btn-submit-login text-uppercase fw-bold"> Đăng nhập</b-button>
                        </div>
                  </div>

                </b-form>
              </div>

            </div>
          </div>
          <!-- end col -->
        </div>
    </div>


</template>

<style lang="scss" >
.account-pages {
  background-image: url("~@/assets/images/bg/background.png");
  background-repeat: no-repeat;
  background-size: cover;
  height: 100vh;
  overflow-x: hidden;

  & .login-box {
    height: 100% !important;

    & .bg-login-box {
      //height: 100%;
      background: transparent;
      margin: auto;
      padding: 20px;
      border-radius: 10px;
      box-shadow: rgba(0, 0, 0, 0.25) 0px 54px 55px, rgba(0, 0, 0, 0.12) 0px -12px 30px, rgba(0, 0, 0, 0.12) 0px 4px 6px, rgba(0, 0, 0, 0.17) 0px 12px 13px, rgba(0, 0, 0, 0.09) 0px -3px 5px;

      & .title-login {
        font-family: 'refault' !important;
        font-size: 28px;
        font-weight: 700;
        color: #fff;
        text-shadow: 1px 1px 2px red, 0 0 1em #1b1bb4, 0 0 0.2em #3535e1;
        //background: -webkit-linear-gradient(90deg, #121a46, #1d6bb8);
        //-webkit-background-clip: text;
        //-webkit-text-fill-color: transparent;
        //-webkit-text-stroke: 1px #1d6bb8;

        @media (max-width: 936px) {
          font-size: 24px;
        }

        @media (max-width: 768px) {
          font-size: 28px;
        }

        @media (max-width: 425px) {
          font-size: 24px;
        }

        @media (max-width: 375px) {
          font-size: 18px;
        }

        @media (max-width: 320px) {
          font-size: 16px;
        }
      }

      & .province {
        font-family: inherit;
        font-weight: 600;
        font-size: 18px;
        color: #fff;
      }

      & .capcha-box {
        & div {
          & div {
            @media (max-width: 320px) {
              width: 240px !important;
            }

            & iframe {
              @media (max-width: 320px) {
                width: 100%;
              }
            }
          }
        }
      }

    }
  }
}

.rc-anchor-content {
  @media (max-width: 320px) {
    display: none;
  }
}

.logo-login>img {
  height: 70px !important;
}

.cs-form-input-login {
  height: 45px;
  background: #ffffff !important;
  border: 1px solid #004aaa !important;
  color: #000000 !important;
}

.cs-form-input-login:hover {
  border: 1px solid #3603a4 !important;
}

.btn-submit-login {
  height: 45px;
  width: fit-content;
  padding: 0px 30px;
  background: #ed8b1c !important;
  color: #fff !important;
}


.btn-submit-login {
  color: #fff;
  padding-right: 20px;
  background-color: #c0392b;
  -webkit-clip-path: polygon(0% 0%, 100% 0, 100% 70%, 90% 100%, 0% 100%);
  clip-path: polygon(0 0, 100% 0, 100% 60%, 80% 100%, 0 100%);

}

.btn-submit-login:hover {
  -webkit-clip-path: polygon(0 0, 100% 0, 100% 100%, 0 100%);
  clip-path: polygon(0 0, 100% 0, 100% 100%, 100% 100%, 0 100%);
}

.btn-submit-login:after {
  content: "\f061";
  color: #e7cd3c;
  font-family: 'Font Awesome 5 Free';
  display: inline-block;
  position: relative;
  right: -55px;
  transition: all 0.2s ease;
}

.btn-submit-login:hover:after {
  margin: -5px 15px;
  right: 0px;
}
</style>
