<script>
import Layout from "../../layouts/main1";
import appConfig from "@/app.config";
import Multiselect from "vue-multiselect";
import { yeucauloiModel } from "@/models/yeucauloiModel";
import { groupModel } from "@/models/groupModel";
import { userModel } from "@/models/userModel";
//import VueUploadMultipleImage from 'vue-upload-multiple-image'
import { required } from "vuelidate/lib/validators";
import urlSlug from 'url-slug'
import { notifyModel } from "@/models/notifyModel";
import Treeselect from "@riophae/vue-treeselect";
import { donViModel } from "@/models/donViModel";
import { labelModel } from "@/models/labelModel";
import { projectModel } from "@/models/projectModel";
//import { stepModel } from "@/models/stepModel";

export default {
  page: {
    title: "Yêu cầu lỗi",
    meta: [{ name: "description", content: appConfig.description }]
  },
  components: {
    Layout,
    Multiselect,
    Treeselect,
    'ckeditor-nuxt': () => {
      return import('@blowstack/ckeditor-nuxt')
    },
  },
  props: {
    modelValue: {
      type: Object,
      required: true
    }
  },
  data() {
    return {
      title: "Yêu cầu lỗi",
      model: yeucauloiModel.baseJson(),
      modeldonVi: donViModel.baseJson(),
      modelgroup: groupModel.baseJson(),
      modeluser: userModel.baseJson(),
      modellabel: labelModel.baseJson(),
      modelproject: projectModel.baseJson(),

      submitted: false,
      editorConfig: {
        toolbar: {
          items: [
            'fontfamily', 'fontsize', '|',
            'uploadImage',
            'code', 'codeBlock', '|',
            'alignment', '|',
            'fontColor', 'fontBackgroundColor', '|',
            'bold', 'italic', 'strikethrough', 'underline', 'subscript', 'superscript', '|',
            'link', '|',
            'bulletedList', 'numberedList', 'todoList', '|',
            'undo', 'redo'
          ],
          shouldNotGroupWhenFull: true
        },
        removePlugins: ['Title', 'ImageCaption'],
        simpleUpload: {
          uploadUrl: process.env.VUE_APP_API_URL + "files/upload-ckeditor",
          headers: {
            'Authorization': 'optional_token'
          }
        },
      },
      apiUrl: process.env.VUE_APP_API_URL,
      url: `${process.env.VUE_APP_API_URL}files/upload`,
      urlView: `${process.env.VUE_APP_API_URL}files/view/`,
      dropzoneOptions: {
        url: `${process.env.VUE_APP_API_URL}files/upload`,
        thumbnailWidth: 300,
        thumbnailHeight: 160,
        maxFiles: 1,
        maxFilesize: 30,
        headers: { "My-Awesome-Header": "header value" },
        addRemoveLinks: true,
        acceptedFiles: ".pdf",
        dropzoneClassName: "dropzonevue-box"
      },
      optionsStatus: [],
      optionsLabel: [],
      optionsGroup: [],
      optionsUser: [],
      optionsProject: [],
      simpleUpload: {
        uploadUrl: process.env.VUE_APP_API_URL + "files/upload-ckeditor",
        headers: {
          'Authorization': 'optional_token'
        }
      },
      isShow: false,
      responseData: {
        resultString: null,
        resultCode: null
      },
      showDeleteModal: false

    };
  },
  validations: {
    model: {
      title: { required },
      description: { required },
      dueDate: { required },
      label: { required },
    },
  },
  async created() {
    this.GetTreeList();
    this.GetNhan();
    this.getStatus();
    this.getGroup();
    this.getUser();
    this.getLabel();
    this.getProject();
    this.handleCreate();

    if (this.$route.params.id) {
      this.getById(this.$route.params.id);
    } else {
      this.model = yeucauloiModel.baseJson();
    }
  },
  watch: {
    model: {
      handler: function (val, oldVal) {
        if (val.title == null) {
          this.model.slug = "";
        }
        else
          this.model.slug = urlSlug(val.title);
      },
      deep: true
    }
  },
  computed: {
    images() {
      if (this.model && this.model.files) {
        let imgs = [];
        this.model.files.map((value, index) => {
          imgs.push({
            id: index,
            fileId: value.fileId,
            path: this.urlView + value.fileId,
            default: 1,
            highlight: 1,
            caption: value.fileName, // Optional
          })
        })

        return imgs;
      }
      return [];
    }
  },
  mounted() {

  },
  methods: {
    Convertdate(date) {
      let newsApiDate = date; // got from the Api
      let timestamp = new Date(newsApiDate).getTime();
      let Day = new Date(timestamp).getDate();
      let Month = new Date(timestamp).getMonth() + 1;
      let Year = new Date(timestamp).getFullYear();
      if (Month >= 10) {
        let OurNewDateFormat = `${Year}-${Month}-${Day}`;
        return OurNewDateFormat;
      } else {
        let OurNewDateFormat = `${Year}-0${Month}-${Day}`;
        return OurNewDateFormat;
      }
    },
    GetDonVi() {
      this.$store.dispatch("donViStore/getTree").then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.listDonvi = res.data;
          let iddv = this.model.donVi;
          const donVi = this.listDonvi.find(x => x.id === iddv);
          if (donVi)
            this.model.donVi = donVi;
        }
      });
    },
    async getById(id) {
      await this.$store.dispatch("yeucauloiStore/getById", id).then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.model = res.data;
          this.GetDonVi();

          this.model.dueDate = this.Convertdate(this.model.dueDate)

        }
      });
    },
    GetYou() {
      // Lấy giá trị từ local storage
      let authUser = JSON.parse(localStorage.getItem("auth-user"));


      this.$store.dispatch("userStore/getAll").then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.listUs = res.data;
          let iduser = authUser.user.id;
          const nuser = [this.listUs.find(p => p.id === iduser)];
          if (nuser)
            this.model.user = nuser;
        }
      });

    },
    handleCreate() {
      let currentProjectLocal = localStorage.getItem('currentProject');
      this.$store.dispatch("projectStore/get").then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.listProject = res.data; //lấy hết cái list
          this.nameproject = JSON.parse(currentProjectLocal);  //test6
          const project = this.listProject.find(p => p.slug === this.nameproject)
          if (project) {
            this.idproject = project.id;
          }
          else {
            this.idproject = null;
          }
          this.model.projectId = this.idproject;
          return;
        }
        this.listProject = [];
      });
    },

    handleSelectionChange(selected) {
      selected.forEach(item => {
        this.model.labels.push(item);
        this.optionsLabel.push(item);
      });
    },
    addTag(newTag) {
      const tag = {
        id: newTag.id,
        name: newTag,
        color: "#FF0000",
        parentId: null,
        knowledge: null,
        isglobal: false,
        idProject: this.idproject
      }

      this.$store.dispatch("labelStore/create", tag).then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.handleShowNotify(res);
          if (this.optionsLabel == null) {
            this.optionsLabel.push(tag);
          }
          this.optionsLabel.push(tag);
        }
      });
      this.model.label.push(tag);
    },
    normalizer(node) {
      if (node.children == null || node.children == 'null') {
        delete node.children;
      }
    },
    async GetTreeList() {
      await this.$store.dispatch("donViStore/getTree").then((res) => {
        this.treeView = res.data;

      })
    },
    async GetNhan() {
      await this.$store.dispatch("labelStore/getTree").then((res) => {
        this.treeView2 = res.data;

      })
    },
    handleShowNotify(res) {
      this.isShow = true;
      this.responseData.resultCode = res.resultCode;
      this.responseData.resultString = res.resultString;
    },
    async getPostById(id) {
      await this.$store.dispatch("yeucauloiStore/getById", id).then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.model = res.data
        }
      });
    },
    async handleSubmit(e) {
      e.preventDefault();
      this.submitted = true;
      this.$v.$touch();
      if (this.$v.$invalid) {
        return;
      } else {
        let loader = this.$loading.show({
          container: this.$refs.formContainer,
        });
        if (
          this.model.id != 0 &&
          this.model.id != null &&
          this.model.id
        ) {
          // Update model
          await this.$store.dispatch("yeucauloiStore/update", this.model).then((res) => {
            if (res.resultCode === 'SUCCESS') {
              this.handleShowNotify(res);
            }
            this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res))
          });
        } else {
          // Create model
          await this.$store.dispatch("yeucauloiStore/create", this.model).then((res) => {
            if (res.resultCode === 'SUCCESS') {
              this.handleShowNotify(res);
            }
            this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res))
          });
        }
        loader.hide();
      }

      this.submitted = false;
    },
    handleHideModal() {
      this.isShow = false;
      this.responseData = {
        resultString: null,
        resultCode: null
      }
    },
    async uploadImageSuccess(formData, index, fileList) {
      await this.$store.dispatch("fileStore/uploadFile", formData).then((res) => {

        if (res.resultCode === 'SUCCESS') {
          var data = res.data;
          if (this.model.files == null)
            this.model.files = [];
          this.model.files = [...this.model.files, { fileId: data.id, fileName: data.fileName }]
          return;
        }
      });
    },
    beforeRemove(index, done, fileList) {

      var fileId = fileList.find(x => x.id == index);
      var r = confirm("Xóa hình ảnh")
      if (r == true) {
        if (this.model && this.model.files && this.model.files.length > 0 && fileId) {
          this.model.files = this.model.files.filter(x => x.fileId != fileId.fileId);

        }
        done();
      }
    },
    async getStatus() {
      await this.$store.dispatch("statusStore/get").then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.optionsStatus = res.data;
          return;
        }
        this.optionsStatus = [];
      });
    },
    async getLabel() {
      let currentProjectLocal = localStorage.getItem('currentProject');
      this.$store.dispatch("projectStore/get").then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.listProject = res.data; //lấy hết cái list
          this.nameproject = JSON.parse(currentProjectLocal);  //test6
          const project = this.listProject.find(p => p.slug === this.nameproject)
          if (project) {
            this.idproject = project.id;
            this.$store.dispatch("labelStore/getwithprojid", this.idproject).then((res) => {
              if (res.resultCode === 'SUCCESS') {
                this.optionsLabel = res.data;
                return;
              }
              this.optionsLabel = [];
            });
          }
        }
      });

    },
    async getGroup() {
      let currentProjectLocal = localStorage.getItem('currentProject');
      this.slug = JSON.parse(currentProjectLocal);
      await this.$store.dispatch("projectStore/getBySlug", this.slug).then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.project = res.data;
          this.optionsGroup = this.project.group;
        }
      });
    },

    async getUser() {
      let currentProjectLocal = localStorage.getItem('currentProject');
      this.slug = JSON.parse(currentProjectLocal);
      await this.$store.dispatch("projectStore/getBySlug", this.slug).then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.project = res.data;
          this.optionsUser = this.project.member;
        }
      });
    },
    async getProject() {
      await this.$store.dispatch("projectStore/get").then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.optionsProject = res.data;

          return;
        }
        this.optionsProject = [];
      });
    },
    async getStep() {
      await this.$store.dispatch("stepStore/get").then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.optionsStep = res.data;
          return;
        }
        this.optionsStep = [];
      });
    },
    handleShowDeleteModal() {
      this.showDeleteModal = true;
    },
    async handleDelete() {
      if (this.model.id != 0 && this.model.id != null && this.model.id) {
        await this.$store.dispatch("yeucauloiStore/delete", this.model.id).then((res) => {
          if (res.resultCode === 'SUCCESS') {
            this.$router.go(-1)
          }
          this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
          // });
        });
      }
    },
  }
};
</script>
<template>
  <Layout>
    <div class="row">
      <div class="col-12">
        <div class="card mb-2">
          <div class="card-body">
            <div class="row">
              <div class="col-md-4 col-12 d-flex align-items-center">
                <h4 class="font-size-18 fw-bold cs-title-page">Yêu cầu lỗi</h4>
                <!-- <button @click="handleCount()">Xem</button> -->
              </div>
              <div class="col-md-8 col-12 text-end">
                <b-button variant="primary" type="button" class="btn w-md btn-primary" @click="$router.go(-1)" size="sm">
                  <i class="mdi mdi-keyboard-backspace me-1"></i> Trở về
                </b-button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
    <div class="row">
      <div class="col-12">
        <div class="card">
          <div class="card-body">
            <form @submit.prevent="handleSubmit" ref="formContainer">
              <div class="row">
                <div class="col-md-7">
                  <div class="col-lg-12 col-md-12 col-12">
                    <div class="mb-2">
                      <label class="form-label cs-title-form" for="validationCustom01"> Tiêu đề:</label>
                      <span class="text-danger">*</span>
                      <input id="validationCustom01" v-model="model.title" type="text" class="form-control" placeholder=""
                        :class="{ 'is-invalid': submitted && $v.model.title.$error, }" @change="handleCreate()" />
                      <div v-if="submitted && !$v.model.title.required" class="invalid-feedback">
                        Tiêu đề không được để trống.
                      </div>
                    </div>
                  </div>
                  <div class="col-md-12">
                    <div class="mb-2">
                      <label class="form-label cs-title-form" for="validationCustom01"> Mô tả:</label>
                      <span class="text-danger">*</span>

                      <ckeditor-nuxt v-model="model.description" :config="editorConfig" />
                      <div v-if="submitted && !$v.model.description.required" class="invalid-feedback">
                        Mô tả không được để trống.
                      </div>


                    </div>
                  </div>

                </div>
                <div class="col-md-5">
                  <div class="row">
                    <div class="col-md-12">
                      <div class="mb-2">
                        <label class="form-label cs-title-form" for="validationCustom01"> Đơn vị: </label>
                        <treeselect :options="treeView" :value="modeldonVi.parentId" :searchable="true" :show-count="true"
                          :default-expand-level="1" placeholder="Chọn đơn vị" v-model="model.donVi">
                          <label slot="option-label"
                            slot-scope="{ node, shouldShowCount, count, labelClassName, countClassName }"
                            :class="labelClassName">
                            {{ node.label }}
                            <span v-if="shouldShowCount" :class="countClassName">({{ count }})</span>
                          </label>
                        </treeselect>
                      </div>
                    </div>
                    <div class="col-md-12">
                      <label class="form-label cs-title-form" for="validationCustom01"> Phân công theo nhóm </label>
                      <multiselect v-model="model.group" :options="optionsGroup" track-by="id" label="name"
                        placeholder="Chọn phân công" deselect-label="Nhấn để xoá" selectLabel="Nhấn enter để chọn"
                        :multiple="true" selectedLabel="Đã chọn"
                        :class="{ 'is-invalid': submitted && $v.model.group.$error, }">
                      </multiselect>
                      <label class="form-label cs-title-form" for="validationCustom01"> Phân công theo người </label>
                      <multiselect v-model="model.user" :options="optionsUser" :multiple="true" track-by="id"
                        label="fullName" placeholder="Chọn thành viên" deselect-label="Nhấn để xoá"
                        selectLabel="Nhấn enter để chọn" selectedLabel="Đã chọn"
                        :class="{ 'is-invalid': submitted && $v.model.user.$error, }"></multiselect>
                      <b-button class="ms-1" v-on:click="GetYou()">Phân công cho tôi</b-button>
                    </div>
                    <div class="col-md-12">
                      <div class="mb-2">
                        <label class="form-label cs-title-form" for="validationCustom01"> Nhãn yêu cầu lỗi: </label>
                        <span class="text-danger">*</span>
                        <multiselect v-model="model.label" :options="optionsLabel" track-by="id" label="name"
                          deselect-label="Nhấn để xoá" selectLabel="Nhấn enter để chọn" selectedLabel="Đã chọn"
                          :multiple="true" :taggable="true" @tag="addTag" tag-placeholder="Thêm mới nhãn này"
                          placeholder="Chọn nhãn hoặc thêm"
                          :class="{ 'is-invalid': submitted && $v.model.label.$error, }">
                        </multiselect>
                        <div v-if="submitted && !$v.model.label.required" class="invalid-feedback">
                          Nhãn dự án không được để trống.
                        </div>

                      </div>
                    </div>

                    <div class="col-md-12">
                      <div class="mb-2">
                        <label class="form-label cs-title-form" for="validationCustom01"> Ngày hoàn thành: </label>
                        <span class="text-danger">*</span>
                        <input id="validationCustom01" v-model="model.dueDate" type="date" class="form-control"
                          placeholder="" :class="{ 'is-invalid': submitted && $v.model.dueDate.$error, }" />
                        <div v-if="submitted && !$v.model.title.required" class="invalid-feedback">
                          Ngày hoàn thành không được để trống.
                        </div>
                      </div>
                    </div>

                    <div class="col-md-12">
                      <div class="mb-2">
                        <label class="form-label cs-title-form" for="validationCustom01"> Trạng thái: </label>
                        <input id="validationCustom01" v-model="model.stepId" type="text" class="form-control"
                          placeholder="" :class="{ 'is-invalid': submitted && $v.model.stepId.$error, }" disabled />
                      </div>

                      <!-- <input id="validationCustom01"  v-model="model.ProjectId" type="text" class="form-control" placeholder=""
                                          :class="{ 'is-invalid': submitted && $v.model.StepId.$error, }" @input="handleCreate()" value="nameproject" /> -->
                      <input type="text" v-model="model.projectId" @input="handleCreate" hidden>
                    </div>


                    <div class="col-md-12">
                      <div class="mb-2">
                        <div class="text-end">
                          <b-button type="submit" variant="primary" class="ms-1" style="width: 100%" size="md"
                            @click="handleSubmit">
                            Lưu yêu cầu
                          </b-button>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>
  </Layout>
</template>
<style scoped>
.title-capso {
  font-weight: bold;
  color: #00568C;
}

.content-capso {
  color: #00568C;
  padding-left: 10px;
}

.capso-container {
  margin-top: 10px;
  display: flex;
  padding: 0px;
}

.hidden-sortable:after,
.hidden-sortable:before {
  display: none !important;
}
</style>
<style lang="scss">
.success-checkmark {
  width: 80px;
  height: 115px;
  margin: 0 auto;

  .check-icon {
    width: 80px;
    height: 80px;
    position: relative;
    border-radius: 50%;
    box-sizing: content-box;
    border: 4px solid #4CAF50;

    &::before {
      top: 3px;
      left: -2px;
      width: 30px;
      transform-origin: 100% 50%;
      border-radius: 100px 0 0 100px;
    }

    &::after {
      top: 0;
      left: 30px;
      width: 60px;
      transform-origin: 0 50%;
      border-radius: 0 100px 100px 0;
      animation: rotate-circle 4.25s ease-in;
    }

    &::before,
    &::after {
      content: '';
      height: 100px;
      position: absolute;
      background: #FFFFFF;
      transform: rotate(-45deg);
    }

    .icon-line {
      height: 5px;
      background-color: #4CAF50;
      display: block;
      border-radius: 2px;
      position: absolute;
      z-index: 10;

      &.line-tip {
        top: 46px;
        left: 14px;
        width: 25px;
        transform: rotate(45deg);
        animation: icon-line-tip 0.75s;
      }

      &.line-long {
        top: 38px;
        right: 8px;
        width: 47px;
        transform: rotate(-45deg);
        animation: icon-line-long 0.75s;
      }
    }

    .icon-circle {
      top: -4px;
      left: -4px;
      z-index: 10;
      width: 80px;
      height: 80px;
      border-radius: 50%;
      position: absolute;
      box-sizing: content-box;
      border: 4px solid rgba(76, 175, 80, .5);
    }

    .icon-fix {
      top: 8px;
      width: 5px;
      left: 26px;
      z-index: 1;
      height: 85px;
      position: absolute;
      transform: rotate(-45deg);
      background-color: #FFFFFF;
    }
  }
}

@keyframes rotate-circle {
  0% {
    transform: rotate(-45deg);
  }

  5% {
    transform: rotate(-45deg);
  }

  12% {
    transform: rotate(-405deg);
  }

  100% {
    transform: rotate(-405deg);
  }
}

@keyframes icon-line-tip {
  0% {
    width: 0;
    left: 1px;
    top: 19px;
  }

  54% {
    width: 0;
    left: 1px;
    top: 19px;
  }

  70% {
    width: 50px;
    left: -8px;
    top: 37px;
  }

  84% {
    width: 17px;
    left: 21px;
    top: 48px;
  }

  100% {
    width: 25px;
    left: 14px;
    top: 45px;
  }
}

@keyframes icon-line-long {
  0% {
    width: 0;
    right: 46px;
    top: 54px;
  }

  65% {
    width: 0;
    right: 46px;
    top: 54px;
  }

  84% {
    width: 55px;
    right: 0px;
    top: 35px;
  }

  100% {
    width: 47px;
    right: 8px;
    top: 38px;
  }
}

.btn-modal {
  background: #00568C;
  border: none;
  color: #fff;
}

.btn-modal:hover {
  background: #00568C;
  border: none;
  color: #fff;
  box-shadow: rgba(50, 50, 93, 0.25) 0px 13px 27px -5px, rgba(0, 0, 0, 0.3) 0px 8px 16px -8px;
}

#my-strictly-unique-vue-upload-multiple-image {
  font-family: 'Avenir', Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
}
</style>

