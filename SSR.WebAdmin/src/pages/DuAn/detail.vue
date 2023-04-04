<script>
import Layout from "../../layouts/main1";
import appConfig from "@/app.config";
import Multiselect from "vue-multiselect";
import { projectModel } from "@/models/projectModel";
import VueUploadMultipleImage from 'vue-upload-multiple-image'
import { required } from "vuelidate/lib/validators";
import urlSlug from 'url-slug'
import { notifyModel } from "@/models/notifyModel";

export default {
  page: {
    title: "Cài đặt dự án",
    meta: [{ name: "description", content: appConfig.description }]
  },
  components: {
    Layout,
    Multiselect,
    VueUploadMultipleImage,
    // 'ckeditor-nuxt': () => {
    //   return import('@blowstack/ckeditor-nuxt')
    // },
  },
  data() {
    return {
      title: "Cài đặt dự án",
      model: projectModel.baseJson(),
      submitted: false,
      editorConfig: {
        toolbar: {
          items: [
            'heading', '|',
            'fontfamily', 'fontsize', '|',
            'uploadImage',
            'code', 'codeBlock', '|',
            'alignment', '|',
            'fontColor', 'fontBackgroundColor', '|',
            'bold', 'italic', 'strikethrough', 'underline', 'subscript', 'superscript', '|',
            'link', '|',
            'outdent', 'indent', '|',
            'bulletedList', 'numberedList', 'todoList', '|',
            'insertTable', '|',
            'undo', 'redo'
          ],
          shouldNotGroupWhenFull: false
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
      optionsUser: [],
      optionsGroup: [],
      optionsLabel: [],
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
      name: {required},
      description: {required},
      // label: {required},
      slug: {required}
    },
  },
  async created() {
    this.getUser();
    this.getGroup();
    this.getLabel();
    // if(this.$route.params.id){
    //   this.getById(this.$route.params.id);
    // }else{
    //   this.model = projectModel.baseJson();
    // }
    if (this.$route.params.slug) {
      this.getBySlug(this.$route.params.slug);
    } else {
      this.model = projectModel.baseJson();
    }
  },
  watch: {
    model: {
      handler: function (val, oldVal) {
        if (val.name == null) {
          this.model.slug = "";
        }
        else
          this.model.slug = urlSlug(val.name);
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
        console.log(imgs);
        return imgs;
      }
      return [];
    }
  },
  mounted() {
  },
  methods: {
    fnGetList() {
      this.$refs.tblList?.refresh()
    },
    handleShowDeleteModal(id) {
      this.model.id = id;
      this.showDeleteModal = true;
    },
    async handleDelete() {
      if (this.model.id != 0 && this.model.id != null && this.model.id) {
        await this.$store.dispatch("projectStore/delete", this.model.id).then((res) => {
          if (res.resultCode==='SUCCESS') {
            this.fnGetList();
            this.showDeleteModal = false;
            this.$router.push("/")
          }
          this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
        });
      }
    },
    handleShowNotify(res) {
      this.isShow = true;
      this.responseData.resultCode = res.resultCode;
      this.responseData.resultString = res.resultString;
    },
    async getById(id) {
      await this.$store.dispatch("projectStore/getById", id).then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.model = res.data
        }
      });
    },
    async getBySlug(slug) {
      await this.$store.dispatch("projectStore/getBySlug", slug).then((res) => {
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
        if (this.model.group == null && this.model.member == null) {
          alert('Thêm nhóm hoặc thành viên cho dự án');
        }
        else {
          if (
            this.model.id != 0 &&
            this.model.id != null &&
            this.model.id
          ) {
            // Update model
            await this.$store.dispatch("projectStore/update", this.model).then((res) => {
              if (res.resultCode === 'SUCCESS') {
                this.fnGetList()
              }
              this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res))
            });
          } 
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
        console.log(res)
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
      console.log(fileList);
      var fileId = fileList.find(x => x.id == index);
      var r = confirm("Xóa hình ảnh")
      if (r == true) {
        if (this.model && this.model.files && this.model.files.length > 0 && fileId) {
          this.model.files = this.model.files.filter(x => x.fileId != fileId.fileId);
          console.log(this.model.files)
        }
        done();
      } else {
        console.log(1)
      }
    },
    async getUser() {
      await this.$store.dispatch("userStore/getAll").then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.optionsUser = res.data;
          return;
        }
        this.optionsUser = [];
      });
    },
    async getGroup() {
      await this.$store.dispatch("groupStore/get").then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.optionsGroup = res.data;
          return;
        }
        this.optionsGroup = [];
      });
    },
    async getLabel() {
      await this.$store.dispatch("labelStore/get").then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.optionsLabel = res.data;
          return;
        }
        this.optionsLabel = [];
      });
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
                <h4 class="font-size-18 fw-bold cs-title-page">Cài đặt dự án</h4>
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
                <div class="col-md-8">
                  <div class="row">
                    <div class="col-lg-12 col-md-12 col-12">
                      <div class="mb-2">
                        <label class="form-label cs-title-form" for="validationCustom01"> Tên dự án </label>
                        <span class="text-danger">*</span>
                        <input id="validationCustom01" v-model="model.name" type="text" class="form-control"
                          placeholder="" :class="{ 'is-invalid': submitted && $v.model.name.$error, }" />
                        <div v-if="submitted && !$v.model.name.required" class="invalid-feedback">
                          Tiêu đề không được để trống.
                        </div>
                      </div>
                    </div>
                    <div class="col-md-12 col-lg-12">
                      <div class="mb-2">
                        <label class="form-label cs-title-form" for="validationCustom01">Mô tả</label>
                        <span class="text-danger">*</span>
                        <textarea 
                        
                        class="form-control" 
                        v-model="model.description" 
                        rows="8"   
                        :class="{'is-invalid': submitted && $v.model.description.$error,}">
                      </textarea>
                        <div
                            v-if="submitted && !$v.model.description.required"
                            class="invalid-feedback"
                        >
                          Mô tả không được để trống.
                        </div>
                      </div>
                    </div>

                    <div class="col-lg-12 col-md-12 col-12">
                      <div class="mb-2">
                        <label class="form-label cs-title-form" for="validationCustom01"> Đường dẫn </label>
                        <span class="text-danger">*</span>
                        <input id="validationCustom01" v-model="model.slug" type="text" class="form-control"
                          placeholder="" :class="{ 'is-invalid': submitted && $v.model.slug.$error, }" />
                        <!-- <div v-if="submitted && !$v.model.slug.required" class="invalid-feedback">
                          Slug không được để trống.
                        </div> -->
                      </div>
                    </div>

                  </div>
                </div>
                <div class="col-md-4">
                  <div class="row">
                    <div class="col-md-12 mb-2">
                      <label class="form-label cs-title-form" for="validationCustom01"> Hình ảnh</label>
                      <div class="col-md-12 d-flex justify-content-center"
                        id="my-strictly-unique-vue-upload-multiple-image">
                        <vue-upload-multiple-image @upload-success="uploadImageSuccess" @before-remove="beforeRemove"
                          :data-images="images" idUpload="myIdUpload" editUpload="myIdEdit" :showEdit="false"
                          class="cs-upload-image"></vue-upload-multiple-image>
                      </div>
                    </div>
                  </div>
                
                
                    <div class="col-md-">
                      <div class="mb-2">
                        <label class="form-label cs-title-form" for="validationCustom01"> Nhóm </label>
                        <multiselect v-model="model.group" :options="optionsGroup" :multiple="true" track-by="id"
                          label="name" placeholder="Chọn nhóm" deselect-label="Nhấn để xoá"
                          selectLabel="Nhấn enter để chọn" selectedLabel="Đã chọn"
                          :class="{ 'is-invalid': submitted && $v.model.group.$error, }"></multiselect>

                      </div>
                    </div>
                  
                  
                    <div class="col-md-12">
                      <div class="mb-2">
                        <label class="form-label cs-title-form" for="validationCustom01"> Thành viên</label>
                        <multiselect v-model="model.member" :options="optionsUser" :multiple="true" track-by="id"
                          label="fullName" placeholder="Chọn thể loại" deselect-label="Nhấn để xoá"
                          selectLabel="Nhấn enter để chọn" selectedLabel="Đã chọn"
                          :class="{ 'is-invalid': submitted && $v.model.member.$error, }"></multiselect>
                      </div>
                    </div>

                  </div>
                
              </div>
              <div class="text-end pt-2">
                <b-button class="btn-delete ms-1 w-md"
                  v-on:click="handleShowDeleteModal(model.id)">Đóng dự án
                </b-button>
                <b-button type="submit" variant="primary" class="ms-1 w-md " style="width: 200px;">Lưu
                </b-button>
                
              </div>
            </form>
            
          </div>
          <b-modal
                v-model="showDeleteModal"
                centered
                title="Xóa dữ liệu"
                title-class="font-18"
                no-close-on-backdrop
            >
              <p>
                Dữ liệu xóa sẽ không được phục hồi!
              </p>
              <template #modal-footer>
                <button v-b-modal.modal-close_visit
                        class="btn btn-outline-info m-1"
                        v-on:click="showDeleteModal = false">
                  Đóng
                </button>
                <button v-b-modal.modal-close_visit
                        class="btn btn-danger btn m-1"
                        v-on:click="handleDelete">
                  Xóa
                </button>
              </template>
            </b-modal>
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
