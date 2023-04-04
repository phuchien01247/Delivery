<script>
import Layout from "../../layouts/main1";
import appConfig from "@/app.config";
import Multiselect from "vue-multiselect";
import { notifyModel } from "@/models/notifyModel";
import { pagingModel } from "@/models/pagingModel";
import { yeucauloiModel } from "@/models/yeucauloiModel";
import { commentModel } from "@/models/commentModel";
import { knowledgeModel } from "@/models/knowledgeModel";
import { required } from "vuelidate/lib/validators";

export default {
  page: {
    title: "Thông tin chi tiết",
    meta: [{ name: "description", content: appConfig.description }]
  },
  components: {
    Layout,
    'ckeditor-nuxt': () => {
      return import('@blowstack/ckeditor-nuxt')
    },
  },
  data() {
    return {
      comments: [],
      isExpanded: false,
      loading: true,
      editorConfig: {
        height: '50px',
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
      title: "Thông tin chi tiết",
      tendonvi: null,
      model: yeucauloiModel.baseJson(),
      modelcomment: commentModel.baseJson(),
      model1: knowledgeModel.baseJson(),
      cmtdata: "",
      submitted: false,
      dateString: 'null',
      isShow: false,
      self: this,
      responseData: {
        resultString: null,
        resultCode: null
      },
      isActive: false,
      isActive1: true,
      showDeleteModal: false,
      showModalXemchitiet:false,
      data: [],
      pagination: pagingModel.baseJson(),
      totalRows: 1,
      todoTotalRows: 1,
      currentPage: 1,
      numberOfElement: 1,
      perPage: 5,
      totalRows1: 1,
      todoTotalRows1: 1,
      currentPage1: 1,
      numberOfElement1: 1,
      perPage1: 5,
      pageOptions: [5, 10, 25, 50, 100],
      filter: null,
      filterOn: [],
      isBusy: false,
      sortDesc: true,
      fieldcomment: [
        {
          key: "title",
          label: "",
          thStyle: { width: '600px' },
        }
      ],
      fields: [
        {
          key: "name",
          label: "",
          sortable: true,
          thStyle: { width: '600px' },
        },
        {
          key: 'process',
          label: '',
          thStyle: { width: '100px', minWidth: '100px' },
        }
      ],
      sortBy: "createdAt"
    };
  },
  validations: {
    model: {
      content: { required }
    },
  },
  async created() {
    this.fnGetList;
    this.toggleActive();
    if (this.$route.params.id) {
      this.getById(this.$route.params.id);
    } else {
      this.model = yeucauloiModel.baseJson();
    }
    this.GetYou();
    this.GetComment();
    this.fetchComments();
    this.fetchActivity();
    this.onEditorReady();
  },
  watch: {
    showModalXemchitiet(status) {
      if (status == false) this.model1 = knowledgeModel.baseJson();
    },
  },
  mounted() {
    this.plainText = this.$refs.myText.innerText;
  },
  methods: {

    onEditorReady() {
      var element = document.getElementsByClassName("ck-editor");
      console.log("element1", element);
      element[0].classList.add("otherclass");
    },
    getNameUser(name) {
      let currentUser = localStorage.getItem('auth-user');
      this.user = JSON.parse(currentUser);
      if (name == this.user.userName) {
        return this.isActive = true;
      }
      else {
        return this.isActive = false;
      }
    },
    async GetHuongdan() {
      this.$store.dispatch("yeucauloiStore/getById", this.$route.params.id).then((res) => {
        this.listissue = res.data;
      })


    },
   
    expandContent() {
      this.isExpanded = true;
    },
  
    async GetComment() {
      let currentProjectLocal = localStorage.getItem('currentProject');
      this.slug = JSON.parse(currentProjectLocal);

      await this.$store.dispatch("commentStore/get").then((res) => {
        this.listcomment = res.data;
      })

      this.$store.dispatch("yeucauloiStore/getById", this.$route.params.id).then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.listissue = res.data;
          //tìm
          const cmt = [this.listcomment.find(p => p.issueId == this.listissue.id)];

          if (cmt) {
            this.$store.dispatch("commentStore/getwithissid", this.listissue.id).then((res) => {
              this.cmtdata = res.data;
            })
          }
          else {
            this.cmtdata = [];
          }
        }
      });
    },
    myProvider(ctx) {
      const params = {
        start: ctx.currentPage,
        limit: ctx.perPage,
        content: this.$route.params.id,
        sortBy: ctx.sortBy,
        sortDesc: ctx.sortDesc,
      }
      this.loading = true
      try {
        return new Promise((resolve, reject) => {
          this.$store.dispatch("commentStore/getPagingParams", params).then(resp => {
            let data = resp.data;
            this.totalRows = data.totalRows
            let items = data.data
            this.numberOfElement = items.length
            this.loading = false
            resolve(items || [])
          }).catch(error => {
            reject(error)
          })
        })
      } finally {
        this.loading = false
      }
    },

    async fetchComments() {
      try {
        const comments = await this.myProvider({
          currentPage: 1,
          perPage: 10,
          sortBy: "date",
          sortDesc: true,
        })
        this.comments = comments
      } catch (error) {
        console.error(error)
      } finally {
        this.loading = false
      }
    },
    async fetchActivity() {
      try {
        const acti = await this.ActProvider({
          currentPage: 1,
          perPage: 10,
          sortBy: "date",
          sortDesc: true,
        })
        this.acti = acti
      } catch (error) {
        console.error(error)
      } finally {
        this.loading = false
      }
    },
    ActProvider(ctx1) {
      const params = {
        start: ctx1.currentPage,
        limit: ctx1.perPage,
        content: this.$route.params.id,
        sortBy: ctx1.sortBy,
        sortDesc: ctx1.sortDesc,
      }
      this.loading = true
      try {
        let promise = this.$store.dispatch("activityStore/getPagingParams", params)
        return promise.then(resp => {
          let data = resp.data;
          this.totalRows1 = data.totalRows
          let items = data.data
          this.numberOfEle9ment1 = items.length
          this.loading = false
          return items || []
        })
      } finally {
        this.loading = false

      }
    },
    GetYou() {
      // Lấy giá trị từ local storage
      let authUser = JSON.parse(localStorage.getItem("auth-user"));
      //console.log(authUser.user.id);

      this.$store.dispatch("userStore/getAll").then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.listUs = res.data;
          let iduser = authUser.user.id;
          const nuser = [this.listUs.find(p => p.id === iduser)];
          const nuser1 = this.listUs.find(p => p.id === iduser);

          if (nuser)
            this.modelcomment.createdBy = nuser1.userName;
          this.full = nuser1.fullName;
          this.modelcomment.issueId = this.$route.params.id;

        }
      });
    },
    async Comment() {
      this.$store.dispatch("commentStore/create", this.modelcomment).then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.handleShowNotify(res);
          // Cập nhật danh sách comment sau khi thêm mới thành công
          this.fetchComments();
          this.ActProvider();
        }
        this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
      });
    },

    async Close() {
      this.model.stepId = "Close"
      // Update model
      await this.$store.dispatch("yeucauloiStore/update", this.model).then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.handleShowNotify(res);
        }
        this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res))
      });
    },
    async Open() {
      this.model.stepId = "Open"
      // Update model
      await this.$store.dispatch("yeucauloiStore/update", this.model).then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.handleShowNotify(res);
        }
        this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res))
      });
    },
    async handleRedirectToDetail(id) {
      await this.$store.dispatch("knowledgeStore/getById", id).then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.model1 = res.data
          this.showModalXemchitiet = true;
          const value = this.model1.summary;
          const div = document.createElement('div');
          div.innerHTML = value;
          const text = div.textContent || div.innerText || '';
          return text;

        } else {
          this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
          this.fnGetList();
        }
      });
    },
    toggleActive() {
      this.$store.dispatch("yeucauloiStore/getById", this.$route.params.id).then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.listIssue = res.data;
          if (this.listIssue.stepId === "Close") {
            this.isActive1 = false;
          }
          else {
            this.isActive1 = true;
          }
        }
      });

    },
    async fnGetList() {
      this.$refs.tblList?.refresh()
    },
    Back() {
      let currentProjectLocal = localStorage.getItem('currentProject');
      this.slug = JSON.parse(currentProjectLocal);
      this.$router.push(`/${this.slug}/danh-sach-yeu-cau-loi/`);
    },
    convertdate(date) {
      let newsApiDate = date; // got from the Api
      let timestamp = new Date(newsApiDate).getTime();
      let Day = new Date(timestamp).getDate();
      let Month = new Date(timestamp).getMonth() + 1;
      let Year = new Date(timestamp).getFullYear();
      let OurNewDateFormat = `${Day}/${Month}/${Year}`;
      return OurNewDateFormat;
    },
    converttime(date) {
      this.lastUpdated = date;
      const lastUpdatedDate = new Date(this.lastUpdated);
      const currentTime = new Date();
      const timeDifferenceInMilliseconds = currentTime.getTime() - lastUpdatedDate.getTime();

      // Tính toán khoảng thời gian giữa thời điểm hiện tại và thời điểm cập nhật lần cuối
      // tính số phút/giờ/ngày/tuần trước
      const timeDifferenceInMinutes = Math.floor(timeDifferenceInMilliseconds / (1000 * 60));
      const timeDifferenceInHours = Math.floor(timeDifferenceInMilliseconds / (1000 * 60 * 60));
      const timeDifferenceInDays = Math.floor(timeDifferenceInMilliseconds / (1000 * 60 * 60 * 24));
      const timeDifferenceInWeeks = Math.floor(timeDifferenceInMilliseconds / (1000 * 60 * 60 * 24 * 7));

      // Chuyển đổi khoảng thời gian thành chuỗi "cách đây ... phút/giờ/ngày/tuần trước"
      if (timeDifferenceInMinutes < 60) {
        this.lastUpdatedText = `${timeDifferenceInMinutes} phút trước`;
      } else if (timeDifferenceInHours < 24) {
        this.lastUpdatedText = `${timeDifferenceInHours} giờ trước`;
      } else if (timeDifferenceInDays < 7) {
        this.lastUpdatedText = `${timeDifferenceInDays} ngày trước`;
      } else {
        this.lastUpdatedText = `${timeDifferenceInWeeks} tuần trước`;
      }
      return this.lastUpdatedText;
    },
    async handleUpdate(id) {
      await this.$store.dispatch("yeucauloiStore/getById", id).then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.model = yeucauloiModel.toJson(res.data);
          let currentProjectLocal = localStorage.getItem('currentProject');
          this.slug = JSON.parse(currentProjectLocal);
          this.$router.push(`/${this.slug}/yeu-cau-loi/${id}`);

        } else {
          this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
        }
      });
    },
    handleShowNotify(res) {
      this.isShow = true;
      this.responseData.resultCode = res.resultCode;
      this.responseData.resultString = res.resultString;
    },
    async getById(id) {
      await this.$store.dispatch("yeucauloiStore/getById", id).then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.model = res.data;
          this.dateString = this.model.dueDate;
          this.$store.dispatch("donViStore/get").then((res) => {
            if (res.resultCode === 'SUCCESS') {
              this.listDonvi = res.data;
              let iddonvi = this.model.donVi;
              const dv = this.listDonvi.find(x => x.id === iddonvi)
              if (dv) {
                this.tendonvi = dv.name;
              } else {
                this.tendonvi = "min";
              }
            }
          });
        }
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
                <h1 class="font-size-18 fw-bold cs-title-page">{{ model.title }}</h1>
                <h4 v-if="isActive1 == true" class="font-size-18 fw-bold cs-title-page" style="padding-left: 20px;"><span
                    class="badge rounded-pill bg-success"><i class="mdi mdi-circle-double"></i> Chờ xử lý</span></h4>
                <h4 v-if="isActive1 == false" class="font-size-18 fw-bold cs-title-page" style="padding-left: 20px;"><span
                    class="badge rounded-pill bg-primary"><i class="mdi mdi-check-circle"></i> Đã hoàn thành</span></h4>

              </div>
              <div class="col-md-8 col-12 text-end">
                <b-button variant="primary" type="button" class="btn w-md btn-primary" size="sm" @click="Back()">
                  <i class="mdi mdi-keyboard-backspace me-1"> Trở về</i>
                </b-button>
                <b-button v-if="isActive1 == true" type="button" variant="danger" class="btn w-md btn-danger" size="sm"
                  @click="Close()" style="height: 30px;">
                  Đóng yêu cầu
                </b-button>
                <button v-if="isActive1 == false" type="button" class="btn" size="sm"
                  style="background-color: green;  color: white; height: 29px;" @click="Open()">
                  Mở lại yêu cầu
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="row">
      <div class="col-12">
        <div class="card mb-2">
          <div class="card-body">
            <div class="col-sm-8">
              <div class="text-sm-end">
                <b-modal v-model="showModalXemchitiet" title="Thông tin chi tiết" title-class="text-black font-18"
                  body-class="p-3" hide-footer centered no-close-on-backdrop size="lg">
                  <form @submit.prevent="handleSubmit" ref="formContainer">
                    <div class="row">
                      <div class="col-12">
                        <div class="mb-3">
                          <label class="text-left" style="color:red">Tên hướng dẫn</label>
                          <input type="hidden" v-model="model1.id" />
                          <p ref="myText" v-html="model1.name"></p>
                        </div>
                      </div>
                      <div class="col-md-12">
                        <div class="mb-2">
                          <label class="text-left" style="color:red">Mô tả ngắn</label>
                          <p ref="myText" v-html="model1.summary"></p>
                          <p>{{ plainText }}</p>
                        </div>
                      </div>
                      <div class="col-md-12">
                        <div class="mb-2">
                          <label class="text-left" style="color:red">Nội dung </label>
                          <p ref="myText" v-html="model1.content"></p>
                          <p>{{ plainText }}</p>
                        </div>
                      </div>
                    </div>
                  </form>
                </b-modal>
              </div>
            </div>
            <form @submit.prevent="handleSubmit" ref="formContainer">
              <div class="row">
                <div class="col-md-7 border-end">
                  <div class="col-lg-12 col-md-12 col-12">
                    <div class="mb-2">
                      <div> Tiêu đề:
                        <label class="form-label cs-title-form" for="validationCustom01"> {{ model.title }}</label>
                      </div>
                    </div>
                    <div class="col-md-6 col-lg-6">
                      <div class="mb-2">
                        <div>Mô tả: </div>
                        <label class="form-label cs-title-form" for="validationCustom01">
                          <div class="desc">
                            <p ref="myText" v-html="model.description" class="cs-img-content-mota"></p>
                            <p>{{ plainText }}</p>
                          </div>
                        </label>
                      </div>
                    </div>
                  </div>
                </div>
                <div class="col-md-5">
                  <div class="row">
                    <div class="col-md-12">
                      <div class="mb-2"><i class="mdi mdi-home"></i>&nbsp;Thuộc đơn vị:<br>
                        <label class="form-label cs-title-form" for="validationCustom01">
                          &nbsp;&nbsp;{{ tendonvi }}
                        </label>
                      </div>
                    </div>
                    <div class="col-md-12">
                      <div class="mb-2">
                        <i class="mdi mdi-account-group"></i>&nbsp;Phân công:<br>
                        <label class="form-label cs-title-form" for="validationCustom01">
                          <div v-for="(value, index) in model.group" :key="index"> &nbsp;
                            {{ value.name }}
                          </div>
                          <div v-for="(value, index) in model.user" :key="index"> &nbsp;
                            {{ value.fullName }}
                          </div>
                        </label>
                      </div>
                    </div>
                    <div class="col-md-12">
                      <div class="mb-2">
                        <i class="mdi mdi-label"></i>&nbsp;Nhãn yêu cầu lỗi:<br>
                        <label class="form-label cs-title-form" for="validationCustom01">
                          <p v-for="(value, index) in model.label" :key="index" style="display: inline-block;">
                            <span style="font-size: 13px; " class="badge rounded-pill"
                              v-bind:style="{ background: value.color }">{{ value.name }}</span>
                              
                            <button v-for="(value1, index) in value.knowledge" :key="index" type="button" size="sm" class="btn btn-detail btn-sm" data-toggle="tooltip"
                              data-placement="bottom" title="Xem chi tiết" v-on:click="handleRedirectToDetail(value1.id)">
                              <i class="fas fas fa-eye"></i>
                            </button>
                          </p>
                        </label>
                      </div>
                    </div>
                    <div class="col-md-12">
                      <div class="mb-2">
                        <i class="mdi mdi-calendar"></i>&nbsp;Hạn hoàn thành:<br>
                        <label class="form-label cs-title-form" for="validationCustom01">
                          <p style="font-size: 15px;">&nbsp;
                            {{ convertdate(model.dueDate) }}</p>
                        </label>
                      </div>
                    </div>
                    <div class="col-md-12 col-12 text-end">
                      <b-button v-if="getNameUser(model.createdBy)" variant="primary" type="button"
                        class="btn w-md btn-primary" size="sm" @click="handleUpdate(model.id)">
                        <i class="mdi mdi-pencil"> Chỉnh sửa</i>
                      </b-button>
                    </div>
                  </div>
                </div>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>
    <div class="row">
      <div class="col-6" style="    padding-right: 2px;">
        <div class="card mb-2">
          <div class="card-body">
            <label class="d-inline-flex align-items-center">
              Hoạt động
            </label>
            <div data-simplebar="init" style="max-height: 500px;">
              <div class="simplebar-wrapper" style="margin: 0px;">
                <div class="simplebar-height-auto-observer-wrapper">
                  <div class="simplebar-height-auto-observer"></div>
                </div>
                <div class="simplebar-mask">
                  <div class="simplebar-offset" style="right: -16.8px; bottom: 0px;">
                    <div class="simplebar-content-wrapper" style="height: auto; overflow: hidden scroll;">
                      <br>
                      <div class="timeline">
                        <ul>
                          <div v-for="act in acti" :key="act.id">
                            <li>
                              <span>
                                <h5 class="font-size-14 mb-1" style="display: initial;">{{ act.createdAtShow }}</h5>
                              </span>
                              <!-- <span>{{ act.createdAtShow }}</span> -->
                              <div class="content">
                                
                                <h3> {{ act.createdBy }} - {{ act.action }}</h3>
                                <p ref="myText" v-html="act.content" class="cs-img-content-binhluan"></p>
                                &nbsp;
                              </div>
                                                    
                                <div v-for="(value, index) in act.label" :key="index" style="font-size: 13px; display: inline-block;" class="badge rounded-pill labelstyle"
                                  v-bind:style="{ background: value.color}">{{ value.name }}
                                </div>                              
                            </li>
                          </div>
                        </ul>
                      </div>
                    </div>
                  </div>
                </div>
                <div class="simplebar-placeholder" style="width: auto; height: 440px;"></div>
              </div>
              <div class="simplebar-track simplebar-horizontal" style="visibility: hidden;">
                <div class="simplebar-scrollbar" style="transform: translate3d(0px, 0px, 0px); display: none;"></div>
              </div>
              <div class="simplebar-track simplebar-vertical" style="visibility: visible;">
                <div class="simplebar-scrollbar"
                  style="height: 218px; transform: translate3d(0px, 91px, 0px); display: block;"></div>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div class="col-6">
        <div class="card">
          <div class="card-body">
            <label class="d-inline-flex align-items-center">
              Bình luận
            </label>

            <div class="chat-conversation">
              <ul class="list-unstyled" data-simplebar="init" style="max-height: 300px;">
                <div class="simplebar-wrapper" style="margin: 0px;">
                  <div class="simplebar-height-auto-observer-wrapper">
                    <div class="simplebar-height-auto-observer"></div>
                  </div>
                  <div class="simplebar-mask">
                    <div class="simplebar-offset" style="right: -16.8px; bottom: 0px;">
                      <div class="simplebar-content-wrapper" style="height: auto; overflow: hidden scroll;">
                        <div class="simplebar-content" style="padding: 0px;">
                          <div v-if="comments.length <= 0 && !isBusy">
                            <div align="center">Không có dữ liệu</div>
                          </div>
                          <div v-else>
                            <ul class="list-group list-group-flush">
                              <li class="list-group-item py-3">
                                <div v-for="comment in comments" :key="comment.id">
                                  <div class="d-flex">
                                    <div class="flex-shrink-0 me-3">
                                      <div class="avatar-xs">
                                        <img src="../../../public/user.png" alt="" height="22">
                                      </div>
                                    </div>

                                    <div class="flex-grow-1">
                                      <div class="chat">
                                        <h5 class="font-size-14 mb-1">{{ comment.createdBy }}
                                        </h5>
                                        <p ref="myText" v-html="comment.content" class="text-muted cs-img-content ellips" :class="{ 'expanded': isExpanded }">                          
                                        </p>
                                        <a v-if="!isExpanded" style="cursor: pointer;" @click="expandContent">Xem thêm</a>
                                      </div>
                                    </div>
                                    <small class="text-muted float-end">{{ converttime(comment.createdAt) }}</small>
                                  </div>
                                </div>
                              </li>
                            </ul>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                  <div class="simplebar-placeholder" style="width: auto; height: 645px;"></div>
                </div>
                <div class="simplebar-track simplebar-horizontal" style="visibility: hidden;">
                  <div class="simplebar-scrollbar" style="transform: translate3d(0px, 0px, 0px); display: none;">
                  </div>
                </div>
                <div class="simplebar-track simplebar-vertical" style="visibility: visible;">
                  <div class="simplebar-scrollbar"
                    style="height: 104px; transform: translate3d(0px, 0px, 0px); display: block;"></div>
                </div>
              </ul>
            </div>


            <div class="chat-input-section">
              <div class="row">
                <div class="col">
                  <div class="position-relative">
                    <div class="col-md-12 col-12 text-end">
                      <input type="text" v-model="modelcomment.issueId" hidden>
                    </div>
                    <ckeditor-nuxt class="ckeditor" v-model="modelcomment.content" :config="editorConfig" />

                  </div>
                </div>
              </div>
              <div class="row" style="margin-left: 70%;">
                <div class="col-auto">
                  <b-button variant="success" type="button" @click="Comment()" size="md">
                    Gửi bình luận <i class="mdi mdi-send"></i>
                  </b-button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </Layout>
</template>
<style scoped>
.ellipsis {
  white-space: nowrap;
  text-overflow: ellipsis;
  overflow: hidden;
}

.ellips {
  display: block;
  display: -webkit-box;
  max-width: 100%;
  margin: 0 auto;
  font-size: 14px;
  line-height: 30px;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
  text-overflow: ellipsis;
}
.expanded {
  -webkit-line-clamp: unset;
}
.title-capso {
  font-weight: bold;
  color: #00568C;
}

.mdi mdi-circle-slice-8 {
  color: blue;
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
// hr.vertical {
//   width: 0px;
//   height: 100%;
//   border-left: 1px solid #000000;
// }

.my-custom {
  height: 20px;
}

.colorstyle {
  padding: 5px;
  border-radius: 5px;
  //font-weight: bold;
  color: white
}

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

@mixin tablet-and-up {
  @media screen and (min-width: 769px) {
    @content;
  }
}

@mixin mobile-and-up {
  @media screen and (min-width: 601px) {
    @content;
  }
}

@mixin tablet-and-down {
  @media screen and (max-width: 768px) {
    @content;
  }
}

@mixin mobile-only {
  @media screen and (max-width: 600px) {
    @content;
  }
}

// .ck.ck-editor {
//   display: block !important;
//   width: 100% !important;
//   padding: 0.375rem 0.75rem !important;
//   font-size: 0.875rem !important;
//   font-weight: 400 !important;
//   line-height: 1.5 !important;
//   color: #495057 !important;
//   background-color: #fff !important;
//   background-clip: padding-box !important;
//   border: 1px solid #ced4da !important;
//   -webkit-appearance: none !important;
//   -moz-appearance: none !important;
//   appearance: none !important;
//   border-radius: 0.25rem !important;
//   position: relative;
//   max-height: 125px !important;
//   max-width: 98% !important;
// }

.border-end {
  border-right: 1px solid #d9d9d9 !important;
  /* đường viền bên phải dày 2px và màu đen */
}

.timeline {
  width: 800px;
  background-color: white;
  padding: 0px 10px 20px 20px;
}

.timeline span {
  width: 800px;
  color: white;
  padding: 30px 20px;
  width: 20%;
}

.timeline ul {
  list-style-type: none;
  border-left: 2px solid #094a68;
  padding: 0px 5px;
}

.timeline ul li {
  padding: 20px 10px 20px;
  position: relative;
  transition: .5s;
}

.timeline ul li span {
  display: inline-block;
  background-color: #d7f2ff;
  border-radius: 25px;
  padding: 2px 5px;
  margin-bottom: -50px !important;
  font-size: 14px;
  text-align: center;
  color: #095c83;
}

.main-content .content {
  padding: 0 15px 15px 10px;
  margin-top: 10px !important;
}

.timeline ul li .content h3 {
  color: #34ace0;
  font-size: 14px;
  margin-top: 5px !important;
  margin-bottom: 5px !important;
}

.timeline ul li .content p {
  font-size: 14px;
  margin-top: -5px !important;
  margin-bottom: -40px !important;
}

.timeline ul li:before {
  position: absolute;
  content: '';
  width: 10px;
  height: 10px;
  background-color: #34ace0;
  border-radius: 50%;
  left: -11px;
  top: 28px;
  transition: .5s;
}

.labelstyle {
  font-size: 15px;
  display: flex;
  width: fit-content;
  margin-left: 10px;
  margin-top: 5px;
}

.chat {
  border: none;
  background: #E2FFE8;
  font-size: 10px;
  border-radius: 10px;
  margin-right: 30px;
  padding: 10px;
  max-width: 420px;
  margin-bottom: 10px;
}

.desc {}

.cs-ckeditor .ck-sticky-panel__content {
  border-top-left-radius: ̀̀5px !important;
  border-top-right-radius: 5px !important;
}

.cs-ckeditor .ck-reset.ck-editor {
  border: 1px solid #ccc !important;
}

.cs-ckeditor .ck-content {
  min-height: 100px !important;
}

.cs-img-content img {
  height: 50px !important;
  width: 100% !important;
  object-fit: cover;
}

.cs-img-content-mota img {
  height: 300px !important;
  object-fit: cover;
}

.cs-img-content-binhluan img {
  height: 200px !important;
  object-fit: cover;
  margin-top: 20px;
}</style>
