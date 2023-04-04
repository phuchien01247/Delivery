<script>
import Layout from "../../layouts/main1";
import appConfig from "@/app.config";
import { projectModel } from "@/models/projectModel";
export default {
  page: {
    title: "Thông tin chi tiết",
    meta: [{ name: "description", content: appConfig.description }]
  },
  components: {
    Layout,
  },
  data() {
    return {
      title: "Thông tin chi tiết",
      model: projectModel.baseJson(),
      submitted: false,
      data: [],
      memdata: [],
      issuedataopen: "",
      issuedataclose: "",
      responseData: {
        resultString: null,
        resultCode: null
      },
      showDeleteModal: false
    };
  },
  validations: {
  },
  async created() {
    this.GetList();
    this.getBySlug();
    /* if (this.$route.params.slug) {
      this.getBySlug(this.$route.params.slug);
    } else {
      this.model = projectModel.baseJson();
    } */

  },
  watch: {
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

    async GetList() {
      let currentProjectLocal = localStorage.getItem('currentProject');
      this.slug = JSON.parse(currentProjectLocal);
      await this.$store.dispatch("yeucauloiStore/get").then((res) => {
        this.listissue = res.data;
        //lấy ds dự án
        this.$store.dispatch("projectStore/getBySlug", this.slug).then((res) => {
          if (res.resultCode === 'SUCCESS') {
            this.listProject = res.data;
            if (this.listProject) {
              this.idproject = this.listProject.id;     //chứa idproject đang mở
            }
            //tìm có idproject giống
            const issue = this.listissue.find(p => p.projectId == this.idproject);

            if (issue) {
              this.$store.dispatch("yeucauloiStore/getopenwithprojid", this.idproject).then((res) => {
                this.issuedataopen = res.data;
              })
              this.$store.dispatch("yeucauloiStore/getclosewithprojid", this.idproject).then((res) => {
                this.issuedataclose = res.data;
              })
            }
          }
        });
      })
    },
    convertdate() {
      let newsApiDate = this.model.createdAt; // got from the Api
      let timestamp = new Date(newsApiDate).getTime();
      let Day = new Date(timestamp).getDate();
      let Month = new Date(timestamp).getMonth() + 1;
      let Year = new Date(timestamp).getFullYear();
      let OurNewDateFormat = `${Day}/${Month}/${Year}`;
      return OurNewDateFormat;
    },
    async getBySlug() {
      let currentProjectLocal = localStorage.getItem('currentProject');
      this.slug = JSON.parse(currentProjectLocal);
      await this.$store.dispatch("projectStore/getBySlug", this.slug).then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.model = res.data;
          this.$store.dispatch("userStore/getAll").then((res) => {
            if (res.resultCode === 'SUCCESS') {
              this.listuser = res.data;
              this.users = this.listuser.find(p => p.userName == this.model.createdBy);
              this.model.createdBy = this.users.fullName;
            }
          });
        }
      });
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
        this.lastUpdatedText = `cách đây ${timeDifferenceInMinutes} phút trước`;
      } else if (timeDifferenceInHours < 24) {
        this.lastUpdatedText = `cách đây ${timeDifferenceInHours} giờ trước`;
      } else if (timeDifferenceInDays < 7) {
        this.lastUpdatedText = `cách đây ${timeDifferenceInDays} ngày trước`;
      } else {
        this.lastUpdatedText = `cách đây ${timeDifferenceInWeeks} tuần trước`;
      }
      return this.lastUpdatedText;
    },
  }
};
</script>

<template>
  <Layout>
    <div class="container-fluid">
      <div class="row">
        <div class="col-12">
          <div class="card mb-2">
            <div class="card-body">
              <div>
                <div class="d-flex mb-12">
                  <div class="col-md-12 col-12 d-flex align-items-center">
                    <img src="@/assets/project.jpg" alt="Generic placeholder image"
                      class="flex-shrink-0 me-3 rounded mx-auto d-blocks avatar-sm">

                    <div class="flex-grow-1">
                      <h4 class="font-size-20 black" style="color: black;">Tên dự án: {{ model.name }}</h4>
                      <router-link :to="`/${slug}/danh-sach-yeu-cau-loi`" style="cursor: pointer;" class="text-muted">
                        <i class="mdi mdi-circle-slice-8" style="color: brown;"></i>
                        <strong>{{ issuedataopen.length }}</strong><span> yêu cầu chờ xử lý</span>
                      </router-link>
                      <router-link :to="`/${slug}/danh-sach-yeu-cau-loi`" style="cursor: pointer; padding-left: 20px;"
                        class="text-muted">
                        <i class="mdi mdi-check-circle" style="color:green;"></i>
                        <strong>{{ issuedataclose.length }} </strong> yêu cầu đã xử lý
                      </router-link>
                    </div>

                  </div>
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
              <div class="d-flex mb-4">
                <img src="@/assets/user.png" alt="Generic placeholder image"
                  class="flex-shrink-0 me-3 rounded-circle avatar-sm">
                <div>
                  <label class="form-label cs-title-form" for="validationCustom01" style="font-size: 15px; ">
                    Người tạo: {{ model.createdBy }}
                  </label> <br>
                  <span class="badge rounded-pill"
                    style="background-color: rgb(161 229 224 / 29%); color:  rgb(2 164 153); font-size: 12px;">Đã tạo
                    {{ converttime(model.createdAt) }}
                  </span>
                </div>

              </div>

              <label class="form-label cs-title-form" for="validationCustom01"> Sơ lược về dự án:
              </label>
              <p class="card-title-desc">{{ model.description }}</p>
              <div class="col-md-12">
                <div class="mb-2">
                  <label class="form-label cs-title-form" for="validationCustom01"> Thành viên:
                  </label><br>
                  <ul class="message-list">
                    <div style="font-size: 17px; ">
                      <span class="badge rounded-pill"
                        style="background-color:rgb(161 229 224 / 29%); color: rgb(2 164 153);"
                        v-for="(value, index) in model.group" :key="index">
                        {{ value.name }}
                      </span>
                      <span class="badge rounded-pill"
                        style="background-color:rgb(161 229 224 / 29%); color: rgb(2 164 153); margin-right: 5px"
                        v-for="(value, index) in model.member" :key="index">
                        {{ value.fullName }}
                      </span>
                    </div>
                  </ul>

                </div>
              </div>
            </div>

          </div>
        </div>
      </div>
      <div class="row">
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
}</style>
