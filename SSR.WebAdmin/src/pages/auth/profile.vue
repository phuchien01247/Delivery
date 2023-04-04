<script>
import Layout from "../../layouts/main";
import PageHeader from "@/components/page-header";
import appConfig from "@/app.config";
import {userModel} from "@/models/userModel";
import {notifyModel} from "@/models/notifyModel";
import {required, sameAs,minLength,maxLength} from "vuelidate/lib/validators";
import VueUploadMultipleImage from 'vue-upload-multiple-image'
import {mapState} from "vuex";

export default {
  page: {
    title: "Thông tin cá nhân",
    meta: [{ name: "description", content: appConfig.description }]
  },
  components: { Layout, PageHeader, VueUploadMultipleImage},

  data() {
    return {
      url: `${process.env.VUE_APP_API_URL}files/upload`,
      url1 :  process.env.VUE_APP_API_URL + 'files/view/' ,
      apiUrl: process.env.VUE_APP_API_URL,
      urlView: `${process.env.VUE_APP_API_URL}files/view/`,
      title: "Thông tin cá nhân",
      items: [
        {
          text: "Thông tin cá nhân",
          href: "/thong-tin-ca-nhan"
        },
        {
          text: "Thông tin",
          active:true
        },
      ],
      data: [],
      modelpass:{
        password:null,
        newPass:null,
        confirmPass:null,
        userName:null,
        id:null
      },
      model: userModel.baseJson(),
      submitted: false,
      submittedPass: false,
      showNotifiModal: false,
      simpleUpload: {
        uploadUrl: process.env.VUE_APP_API_URL + "files/upload-ckeditor",
        headers: {
          'Authorization': 'optional_token'
        }
      },
    };
  },
  created() {
    this.handleProfile();
    this.getInfomation(this.handleProfile().user.id);
    this.getID();
  },
  computed: {
    ...mapState('userStore', ['reloadAuthUser']),
    images() {
      if(this.model && this.model.avatar){
        let imgs = [];
        this.model.avatar.map((value, index) =>{
          imgs.push({
            id: index,
            fileId: value.fileId,
            path: this.urlView + value.fileId,
            default: 1,
            highlight: 1,
            caption: value.fileName, // Optional
          })
        })
        console.log(imgs);
        return imgs;
      }
      return [];
    }
  },
  watch:{
    reloadAuthUser(value) {
      this.loadData();
    },
  },
  validations: {
    model: {
      firstName: {required},
      lastName: {required},
    },
    modelpass: {
      password: { required },
      newPass:{required,
        valid: function(value) {
          const containsUppercase = /[A-Z]/.test(value)
          const containsLowercase = /[a-z]/.test(value)
          const containsNumber = /[0-9]/.test(value)
          const containsSpecial = /[#?!@$%^&*-]/.test(value)
          return containsUppercase && containsLowercase && containsNumber && containsSpecial
        },
        minLength: minLength(8),
        maxLength: maxLength(20),
      },
      confirmPass: { required, sameAsPassword: sameAs('newPass') }
    }
  },
  methods:{
    async getInfomation(id){
      await  this.$store.dispatch("userStore/getById",id).then((res) =>{
        console.log(res.data)
        this.model = res.data
      })
    },
    async loadData() {
      await this.getInfomation();
      this.$refs.tree.setModel(this.data)
    },
    handleProfile() {
      const profile = localStorage.getItem("auth-user");
      return JSON.parse(profile);
    },
    async handleSubmit(e) {
      e.preventDefault();
      this.submitted = true;
      this.$v.$touch();
      if (this.$v.model.$invalid) {
        return;
      } else {
        let loader = this.$loading.show({
          container: this.$refs.formContainer
        });
        if (
            this.model.id != 0 &&
            this.model.id != null &&
            this.model.id
        ) {
          // Update model
          await this.$store.dispatch("userStore/changeProfile", this.model).then((res) => {
            if (res.success) {
              this.$store.commit('userStore/SET_RELOADAUTHUSER', !this.$store.state.userStore.reloadAuthUser)
              // localStorage.removeItem('auth-user');
              // localStorage.setItem('auth-user', JSON.stringify(res.data));
            }
            this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res))
          });
        }
        loader.hide();
      }
      this.submitted = false;
    },
    getID(){
      const idstore= localStorage.getItem('auth-user');
      this.modelpass.userName=JSON.parse(idstore)?.user?.userName;
      this.modelpass.id=JSON.parse(idstore)?.user?.id;
      return JSON.parse(idstore);
    },
    async handleDetail(id) {
      await this.$store.dispatch("userStore/getById", id).then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.model = userModel.fromJson(res.data);
          this.showDetail = true;
        } else {
          this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
        }
      });
    },
    async handleSubmitPass(e) {
      e.preventDefault();
      this.submittedPass = true;
      this.$v.$touch()
      if (this.$v.modelpass.$invalid) {
        return;
      } else {
        let loader = this.$loading.show({
          container: this.$refs.formPasswordContainer,
        });
        if (
            this.modelpass.id != 0 &&
            this.modelpass.id != null &&
            this.modelpass.id
        ) {
          await this.$store.dispatch("userStore/changePassword", this.modelpass).then((res) => {
            if (res.success) {
              this.fnGetList();
              this.showModal = false;
            }
            this.modelpass.newPass =null;
            this.modelpass.password=null;
            this.modelpass.confirmPass=null;
            if(res.resultCode=='SUCCESS'){
              this.showNotifiModal=true;
            }
            else{
              this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res))
            }
          });
        }
        loader.hide();
      }
      this.submittedPass = false;
    },
    async uploadImageSuccess(formData, index, fileList) {
      await this.$store.dispatch("fileStore/uploadFile", formData).then((res) => {
        console.log(res)
        if (res.resultCode === 'SUCCESS') {
          var data = res.data;
          if(this.model.avatar == null)
            this.model.avatar = [];
          this.model.avatar = [...this.model.avatar,  {fileId: data.id, fileName: data.fileName}]
          return;
        }
      });
    },
    beforeRemove (index, done, fileList) {
      console.log(fileList);
      var fileId = fileList.find(x => x.id == index);
      var r = confirm("Xóa hình ảnh")
      if (r == true) {
        if(this.model && this.model.avatar && this.model.avatar.length > 0 && fileId){
          this.model.avatar = this.model.avatar.filter(x => x.fileId != fileId.fileId);
          console.log(this.model.avatar)
        }
        done();
      } else {
        console.log(1)
      }
    },
    logoutUser() {
      // eslint-disable-next-line no-unused-vars
      var userLocalStorage = localStorage.getItem("user-token");
      if (userLocalStorage) {
        localStorage.removeItem("user-token");
        localStorage.removeItem("auth-user");
        localStorage.removeItem("TabData");
        window.location.href="/dang-nhap"
        // this.$router.push("/dang-nhap");
        return;
      }
    },
  },

};
</script>

<template>
  <Layout>
    <PageHeader :title="title" :items="items" />
    <div class="row">
      <div class="col-xl-4">
        <div class="card overflow-hidden">
          <div class="bg-soft bg-primary">
            <div class="row">
              <div class="col-7">
                <div class="text-primary p-2">
                  <h5 class="text-primary">Chào mừng bạn đã trở lại</h5>
                </div>
              </div>
              <div class="col-5 align-self-end">
                <img src="@/assets/images/profile-img.png" alt class="img-fluid" />
              </div>
            </div>
          </div>
          <div class="card-body pt-0" style="margin-top: -10px">
            <div class="row">
              <div class="col-sm-12">
                <div class="avatar-md profile-user-wid mb-2">
                  <div v-if="model.avatar != null">
                    <img 
                      :src="url +`${model.avatar.fileId}`"
                      class="img-thumbnail rounded-circle"
                      style="height: 72px ; width: 72px"
                    >
                  </div>
                  <div v-else>
                    <img
                        class="img-thumbnail rounded-circle"
                        style="height: 72px ; width: 72px"
                        alt
                        src="@/assets/user.png"
                    />
                  </div>
                </div>
                <h5 class="font-size-15 text-dark fw-bold">{{model.lastName+" "+model.firstName}}</h5>
                <p class="mb-0 text-dark">@{{model.userName}}</p>
              </div>
            </div>
          </div>
        </div>
        <!-- end card -->
        <div class="card">
          <div class="card-body">
            <h4 class="card-title mb-4">Thông tin cá nhân</h4>

         
            <div class="table-responsive ">
              <table class="table table-nowrap mb-0">
                <tbody>
                <tr>
                  <th scope="row">Tên tài khoản :</th>
                  <td>{{model.userName}}</td>
                </tr>
                <tr>
                  <th scope="row">Họ và tên :</th>
                  <td>{{model.lastName +" "+ model.firstName}}</td>
                </tr>
                <tr>
                  <th scope="row">E-mail :</th>
                  <td>{{model.email}}</td>
                </tr>
                <tr>
                  <th scope="row">Số điện thoại :</th>
                  <td>{{model.phoneNumber}}</td>
                </tr>
                <tr>
                  <th scope="row">Quyền :</th>
                  <td>
                    <div v-for="item in model.role" v-bind:key="item.id">{{item.name}}</div></td>
                </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
        <!-- end card -->
      </div>

      <div class="col-xl-8">
        <div class="col-xl-12">
          <div class="card">
            <div class="card-body">
              <b-tabs justified nav-class="nav-tabs-custom" content-class="p-3 text-muted">
                <b-tab active>
                  <template v-slot:title>
                  <span class="d-inline-block d-sm-none">
                    <i class="far fa-user"></i>
                  </span>
                    <span class="d-none d-sm-inline-block">Thông tin tài khoản</span>
                  </template>
                  <div class="text-end">
                    <b-button type="submit" variant="primary" class=" align-middle me-2" style="width: 80px" size="sm"
                              @click="handleSubmit"
                    >
                      Cập nhật
                    </b-button>
                  </div>
                  <form @submit.prevent="handleSubmit" ref="formContainer">
                    <div class="row">
                      <div class="col-6">
                        <div class="mb-3">
                          <label class="text-left">Họ</label>
                          <span style="color: red">&nbsp;*</span>
                          <input type="hidden" v-model="model.id"/>
                          <input
                              id="lastName"
                              v-model="model.lastName"
                              type="text"
                              class="form-control"
                              placeholder="Nhập họ"
                              :class="{
                                'is-invalid':
                                  submitted && $v.model.lastName.$error,
                              }"
                          />
                          <div
                              v-if="submitted && !$v.model.lastName.required"
                              class="invalid-feedback"
                          >
                            Họ không được trống.
                          </div>
                        </div>
                      </div>
                      <div class="col-6">
                        <div class="mb-3">
                          <label class="text-left">Tên</label>
                          <span style="color: red">&nbsp;*</span>
                          <input type="hidden" v-model="model.id"/>
                          <input
                              id="firstName"
                              v-model="model.firstName"
                              type="text"
                              class="form-control"
                              placeholder="Nhập tên"
                              :class="{
                                'is-invalid':
                                  submitted && $v.model.firstName.$error,
                              }"
                          />
                          <div
                              v-if="submitted && !$v.model.firstName.required"
                              class="invalid-feedback"
                          >
                            Tên không được trống.
                          </div>
                        </div>
                      </div>
                      <div class="col-6">
                        <div class="mb-3">
                          <label class="text-left">Số điện thoại</label>

                          <input
                              id="phoneNumber"
                              v-model="model.phoneNumber"
                              v-mask="'##########'"
                              type="text"
                              class="form-control"
                              placeholder="Nhập số điện thoại"
                          />
                        </div>
                      </div>
                      <div class="col-6">
                        <div class="mb-3">
                          <label class="text-left">Email</label>
                          <input
                              id="email"
                              v-model="model.email"
                              type="email"
                              pattern="[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}$"
                              class="form-control"
                              placeholder="Nhập email"
                          />
                        </div>
                      </div>
                      <div class="col-6">
                        <label class="text-left"> Ảnh cá nhân </label>
                        <div class=" profile-user-wid mt-2">
                          <div v-if="model.avatar != null">
                            <b-img
                                style="height: 200px ; width: 300px"
                                :src=" url1 + `${model.avatar.fileId}`">
                            </b-img>
                          </div>
                          <div v-else>
                            <img
                                style="max-height: 280px ; width: 300px"
                                src="@/assets/user.png"
                            />
                          </div>
                        </div>
                      </div>

                      <div class="col-6" >
                        <label class="text-left">Chọn ảnh </label>
                        <input type="hidden" v-model="model.id"/>
                        <vue-upload-multiple-image
                            @upload-success="uploadImageSuccess"
                            @before-remove="beforeRemove"
                            :data-images="images"
                            idUpload="myIdUpload"
                            editUpload="myIdEdit"
                            :showEdit="false"
                            class="cs-upload-image"
                        ></vue-upload-multiple-image>
                      </div>
                    </div>
                  </form>

                </b-tab>
                <b-tab>
                  <template v-slot:title>
                  <span class="d-inline-block d-sm-none">
                    <i class="fas fa-key"></i>
                  </span>
                    <span class="d-none d-sm-inline-block">Đổi mật khẩu</span>
                  </template>
                  <form @submit.prevent="handleSubmitPass"  ref="formPasswordContainer">
                    <div class="row">
                      <div class="col-6">
                        <div class="mb-3">
                          <label class="text-left">Tên tài khoản</label>
                          <input type="hidden" v-model="modelpass.id"/>
                          <input
                              id="name"
                              type="text"
                              class="form-control"
                              v-model= "modelpass.userName"
                              disabled
                          />
                        </div>
                      </div>
                      <div class="col-6">
                        <div class="mb-3">
                          <label class="text-left">Mật khẩu</label>
                          <span style="color: red">&nbsp;*</span>
                          <input type="hidden" v-model="modelpass.id"/>
                          <input
                              v-model="modelpass.password"
                              id="pwd"
                              type="password"
                              name="pwd"
                              class="form-control"
                              placeholder="Nhập mật khẩu"
                              :class="{
                                'is-invalid':
                                  submittedPass && $v.modelpass.password.$error,
                              }"
                          />
                          <div
                              v-if="submittedPass && !$v.modelpass.password.required"
                              class="invalid-feedback"
                          >
                            Mật khẩu không được để trống.
                          </div>
                        </div>
                      </div>
                      <div class="col-6">
                        <div class="mb-3">
                          <label class="text-left">Mật khẩu mới</label>
                          <span style="color: red">&nbsp;*</span>
                          <input type="hidden" v-model="modelpass.id"/>
                          <input
                              v-model="modelpass.newPass"
                              id="pwdnew"
                              type="password"
                              class="form-control"
                              placeholder="Nhập mật khẩu mới"
                              :class="{
                                'is-invalid':
                                  submittedPass && $v.modelpass.newPass.$error,
                              }"
                          />
                          <div
                              v-if="submittedPass && !$v.modelpass.newPass.required"
                              class="invalid-feedback"
                          >
                            Mật khẩu mới không được để trống.
                          </div>
                          <div
                              v-if="submittedPass && !$v.modelpass.newPass.valid"
                              class="invalid-feedback"
                          >
                            Mật khẩu cần có ít nhất một ký tự chữ hoa, một ký tự chữ thường, một số, một ký tự đặc biệt.
                          </div>
                          <div
                              v-if="submittedPass && !$v.modelpass.newPass.minLength"
                              class="invalid-feedback"
                          >
                            Mật khẩu cần có ít nhất 8 ký tự.
                          </div>
                          <div
                              v-if="submittedPass && !$v.modelpass.newPass.maxLength"
                              class="invalid-feedback"
                          >
                            Mật khẩu chỉ được tối đa 20 ký tự.
                          </div>
                        </div>
                      </div>
                      <div class="col-6">
                        <div class="mb-3">
                          <label class="text-left">Nhập lại mật khẩu</label>
                          <span style="color: red">&nbsp;*</span>
                          <input type="hidden" v-model="modelpass.id"/>
                          <input
                              v-model="modelpass.confirmPass"
                              id="confirmpwd"
                              type="password"
                              class="form-control"
                              placeholder="Nhập lại mật khẩu"
                              :class="{
                                'is-invalid':
                                  submittedPass && $v.modelpass.confirmPass.$error,
                              }"
                          />
                          <div
                              v-if="submittedPass && !$v.modelpass.confirmPass.required"
                              class="invalid-feedback"
                          >
                            Vui lòng nhập lại mật khẩu.
                          </div>
                          <div
                              v-if="submittedPass &&modelpass.confirmPass && !$v.modelpass.confirmPass.sameAsPassword"
                              class="invalid-feedback"
                          >
                            Nhập lại mật khẩu và mật khẩu mới không trùng khớp.
                          </div>

                        </div>
                      </div>

                      <div class="text-end pt-2 mt-3">
                        <b-button  type="submit" variant="success" class="ms-1 px-3" size="sm">
                          <i class="mdi mdi-check-bold align-middle me-2"></i>
                          Lưu
                        </b-button>
                      </div>
                    </div>
                  </form>
                </b-tab>

              </b-tabs>
            </div>
          </div>
        </div>
      </div>

    </div>
    <!-- end row -->
    <b-modal
        v-model="showNotifiModal"
        centered
        title="Thông tin"
        title-class="font-18; card-title"
        no-close-on-backdrop
        hide-header-close
        size="sm"
    >
      <p>
        Cập nhật mật khẩu thành công!
      </p>
      <template #modal-footer>
        <button v-b-modal.modal-close_visit
                class="btn btn-success btn m-1"
                v-on:click="logoutUser">
          OK
        </button>
      </template>
    </b-modal>
  </Layout>
</template>
