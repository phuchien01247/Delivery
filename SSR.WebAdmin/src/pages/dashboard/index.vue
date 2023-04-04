<script>
import Layout from "../../layouts/main";
import appConfig from "@/app.config";
import Stat from "@/components/widgets/widget-stat";
import {sparklineChartData, salesDonutChart, radialBarChart} from "./data";
import {CONSTANTS} from "@/helpers/constants";
import {dashboardModel} from "@/models/dashboardModel";
import {pagingModel} from "@/models/pagingModel";
import {projectModel} from "@/models/projectModel";
import {notifyModel} from "@/models/notifyModel";
export default {
  page: {
    title: "Bảng điều khiển",
    meta: [{name: "description", content: appConfig.description}]
  },
  components: {
    Layout,
    Stat
  },
  data() {
    return {
      sparklineChartData: sparklineChartData,
      salesDonutChart: salesDonutChart,
      radialBarChart: radialBarChart,
      data: [],
      showModal: false,
      showDetail: false,
      showDeleteModal: false,
      submitted: false,
      pagination: pagingModel.baseJson(),
      totalRows: 1,
      todoTotalRows: 1,
      currentPage: 1,
      numberOfElement: 5,
      perPage: 5,
      pageOptions: [5],
      filter: null,
      filterOn: [],
      isBusy: false,
      sortBy: "date",
      sortDesc: true,
      model: projectModel.baseJson(),
      itemFilter:{
        code: null,
        name: null,
      },
      items: [
        {
          text: "Dự án",
          href:"/du-an",
          // active: true,
        },
        {
          text: "Danh sách",
          active: true,
        }
      ],
      
      fields: [
      {
          key: 'STT',
          label: 'STT',
          class: 'cs-text-center',
          sortable: false,
          thClass: 'hidden-sortable',
          thStyle: { width: '40px', minWidth: '60px' }
        },
        {
          key: "name",
          label: "Dự án",
          sortable: true,
          thClass: 'hidden-sortable',
          thStyle: { width: '600px', minWidth: '200px'},
        },
        {
          key: "createdAt",
          label: "Ngày tạo",
          sortable: true,
          thClass: 'hidden-sortable',
          thStyle: { width: '60px'},
        },
        {
          key: "createdBy",
          label: "Người tạo",
          sortable: true,
          thClass: 'hidden-sortable',
          thStyle: { width: '90px'},
        },
        {
          key: 'process',
          label: 'Xử lý',
          class: 'td-xuly btn-process',
          thClass: 'hidden-sortable',
          // sortable: false,
          thStyle: { width: '80px', minWidth: '50px' },
        }
      ],
      statData: [
     
        {
          title: "Số dự án đã tham gia",
          image: require("@/assets/images/services-icon/02.png"),
          value: "0",
          subText: "VBD",
          color: "white",
          path: "/du-an",
          className: "card-vanbandi",
          icon: "mdi-book-arrow-left text-success"
        },
        {
          title: "Yêu cầu chưa giải quyết",
          image: require("@/assets/images/services-icon/03.png"),
          value: "0",
          subText: "VBDXL",
          color: "white",
          path: "/loi-can-xu-ly",
          className: "card-hopthu",
          icon: "mdi mdi-file-alert"
        },
        {
          title: "Yêu cầu cần giải quyết trong ngày",
          image: require("@/assets/images/services-icon/04.png"),
          value: "0",
          subText: "HT",
          color: "white",
          path: "/loi-trong-ngay",
          className: "card-thongbao",
          icon: "mdi-alert text-warning"
        },
        {
          title: "Yêu cầu đã giải quyết trong ngày",
          image: require("@/assets/images/services-icon/04.png"),
          value: "0",
          subText: "HT",
          color: "white",
          path: "/loi-da-giai-quyet",
          className: "card-vanbanden",
          icon: "mdi mdi-file-check"
        },
       
      ],
      modelSoLieu: dashboardModel.baseJson(),
      activityUserData: [],
      userLogins: []
    };
  },
  async created() {
    this.soLieuDashboard();
    this.getUser();
    this.getGroup();
    this.getLabel();
    this.handleGetDonVi();
    if(this.$route.params.id){
      this.getById(this.$route.params.id);
    }else{
      this.model = projectModel.baseJson();
    }
    if(this.$route.params.slug){
      this.getBySlug(this.$route.params.slug);
    }else{
      this.model = projectModel.baseJson();
    }
  },
  methods: {
    convertdate(date) {
      let newsApiDate = date; // got from the Api
      let timestamp = new Date(newsApiDate).getTime();
      let Day = new Date(timestamp).getDate();
      let Month = new Date(timestamp).getMonth() + 1;
      let Year = new Date(timestamp).getFullYear();
      let OurNewDateFormat = `${Day}/${Month}/${Year}`;
      return OurNewDateFormat;
    },
    soLieuDashboard() {
      try {
        this.$store.dispatch("dashboardStore/getDashboard").then(resp => {
          if (resp.resultCode == CONSTANTS.SUCCESS) {
            this.modelSoLieu = resp.data;
            if (this.modelSoLieu) {
              this.statData[0].value = this.modelSoLieu.soProject.toString();
              this.statData[1].value = this.modelSoLieu.soLoiCanGiaiQuyet.toString();
              this.statData[2].value = this.modelSoLieu.soLoiTrongNgay.toString();
              this.statData[3].value = this.modelSoLieu.soLoiDaGiaiQuyet.toString();
             
            }
            return;
          } else {
            return null;
          }
        })
      } finally {
        console.log()
      }
    },
    getNameUser(name) {              
        let user = this.listuser.find(x => x.userName == name);
          return user.fullName;
    },
    handleGetDonVi() {
      this.$store.dispatch("userStore/getAll").then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.listuser = res.data;
        } 
      });
    },
  async getUser(){
      await this.$store.dispatch("userStore/getAll").then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.optionsUser = res.data;
          return;
        }
        this.optionsUser = [];
      });
    },

    async getGroup(){
      await this.$store.dispatch("groupStore/get").then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.optionsGroup = res.data;
          return;
        }
        this.optionsGroup = [];
      });
    },
    async getLabel(){
      await this.$store.dispatch("labelStore/get").then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.optionsLabel = res.data;
          return;
        }
        this.optionsLabel = [];
      });
    },
    async handleDelete() {
      if (this.model.id != 0 && this.model.id != null && this.model.id) {
        await this.$store.dispatch("projectStore/delete", this.model.id).then((res) => {
          if (res.resultCode==='SUCCESS') {
            this.fnGetList();
            this.showDeleteModal = false;
          }
          this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
        });
      }
    },
    handleShowDeleteModal(id) {
      this.model.id = id;
      this.showDeleteModal = true;
    },
    async handleUpdate(slug) {
      await this.$store.dispatch("projectStore/getBySlug", slug).then((res) => {
        this.$router.push("")
        if (res.resultCode==='SUCCESS') {
          this.model = projectModel.toJson(res.data);
          this.showModal = true;
        } else {
          this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
        }
      });
    },
    myProvider (ctx) {
      const params = {
        start: ctx.currentPage,
        limit: ctx.perPage,
        content: this.filter,
        sortBy: ctx.sortBy,
        sortDesc: ctx.sortDesc,
      }
      this.loading = true
      try {
        let promise =  this.$store.dispatch("projectStore/getPagingParams", params)
        return promise.then(resp => {
          let items = resp.data.data
          this.totalRows = resp.data.totalRows
          this.numberOfElement = resp.data.data.length
          this.loading = false
          return items || []
        })
      } finally {
        this.loading = false
      }
    },
    
    base64ToArrayBuffer(base64) {
      var binaryString = window.atob(base64);
      var binaryLen = binaryString.length;
      var bytes = new Uint8Array(binaryLen);
      for (var i = 0; i < binaryLen; i++) {
        var ascii = binaryString.charCodeAt(i);
        bytes[i] = ascii;
      }
      return bytes;
    },
    handleDetail(slug) {
      this.$router.push(`/${slug}/chi-tiet`)
    },
    handleDetailProject(slug) {

let currentProject = JSON.stringify(slug)

localStorage.setItem("currentProject", currentProject);

this.$router.push(`/${slug}/danh-sach-yeu-cau-loi`)
}
  }
};
</script>

<template>
  <Layout>
    <!-- start page title -->
    <div class="row align-items-center">
      <div class="col-sm-6">
        <div class="page-title-box p-2 mb-2">
          <h4 class="font-size-18 text-dark"> Bảng điều khiển</h4>
          <ol class="breadcrumb mb-0">
            <li class="breadcrumb-item active text-dark"> Chào mừng đến với hệ thống Hỗ trợ người dùng</li>
          </ol>
        </div>
      </div>
        
    </div>
    <!-- end page title -->
    <div class="row mb-3">
      <div class="col-md-12 col-sm-12 mb-3 col-lg-3 center" v-for="stat of statData" :key="stat.icon">
        <Stat
            :title="stat.title"
            :image="stat.image"
            :subText="stat.subText"
            :value="stat.value"
            :color="stat.color"
            :path="stat.path"
            :icon="stat.icon"
            :className="stat.className"
        />
      </div>
    </div>
    <div class="row">
      <div class="col-12">
        <div class="card">
          <div class="card-body" style="margin-top: -20px;">
            <!-- <div class="col-sm-4">
                <div class="search-box me-2 mb-2 d-inline-block">
                  <div class="position-relative">
                    <input
                        v-model = "filter"
                        type="text"
                        class="form-control"
                        placeholder="Tìm kiếm ..."
                    />
                    <i class="bx bx-search-alt search-icon"></i>
                  </div>
                </div>
              </div> -->
              <div class="row">
              <div class="col-12">
                <div class="row mb-3">
                  <div class="col-sm-12 col-md-6">
                    <div id="tickets-table_length" class="dataTables_length">
                      
                    </div>
                  </div>
                </div>
                <div class="table-responsive mb-0">
                    <b-table
                      class="datatables custom-table"
                      :items="myProvider"
                      :fields="fields"
                      responsive="sm"
                      :per-page="perPage"
                      :current-page="currentPage"
                      :sort-by.sync="sortBy"
                      :sort-desc.sync="sortDesc"
                      :filter="filter"
                      :filter-included-fields="filterOn"
                      ref="tblList"
                      primary-key="id"
                      :busy.sync="isBusy"
                      tbody-tr-class="b-table-chucvu linkcolor"
                  >
                  <template v-slot:cell(STT)="data">
                      
                      {{ data.index + ((currentPage-1)*perPage) + 1  }}
                    </template>
                    <template v-slot:cell(name)="data">                  
                      <a style="cursor: pointer;" v-on:click="handleDetailProject(data.item.slug)">
                        <div>
                          <b style="font-size: 17px; color: black;"> {{ data.item.name }}</b>
                          <div class="text-truncate" style="color: dimgray;max-width: 650px;">{{ data.item.description }}</div>
                        </div>
                      </a>
                    </template>
                    <template v-slot:cell(createdAt)="data">                  
                          <p style="font-size: 15px; color: black;">{{ convertdate(data.item.createdAt) }}</p>
                    </template>  
                    <template v-slot:cell(createdBy)="data">                  
                          <p style="font-size: 15px; color: black;">{{ getNameUser(data.item.createdBy)}}</p>
                    </template>  
                    <template v-slot:cell(process)="data">
                      <button
                          type="button"
                          size="sm"
                          class="btn btn-detail btn-sm"
                          data-toggle="tooltip" data-placement="bottom" title="Chi tiết"
                          v-on:click="handleDetail(data.item.slug)">
                        <i class="fas fa-eye "></i>
                      </button>
                    </template>
                  </b-table>
                  <template v-if="isBusy">
                    <div align="center">Đang tải dữ liệu</div>
                  </template>
                  <template v-if="totalRows <= 0 && !isBusy">
                    <div align="center">Không có dữ liệu</div>
                  </template>
                </div>
                <div class="row">
                  <b-col>
                    <div>Hiển thị 5 dự án mới nhất</div>
                  </b-col>
                  
                </div>

              </div>
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
    </div>
<!--    <div class="row">-->
<!--      <div class="col-lg-4 col-md-12">-->
<!--        <Activity :activityUserData="userLogins" />-->
<!--      </div>-->
<!--    </div>-->
  </Layout>
</template>

<style>
.title-capso {
  font-weight: bold;
  color: #00568C;
}

.pd-custom-2 {
  padding: 0px 5px;
}
</style>

<style lang="scss">
.card-vanbanden {
  //background: url("~@/assets/images/card-vanbanden.png");
  //background-repeat: no-repeat;
  //background-size: cover;
  background: linear-gradient(135deg, #308dc7, #6dbdf3);
  color: #fff;
  min-height: 120px !important;
  height: 100%;

  & .mini-stat-label {
    color: #2a2a2a !important;
  }

}


.card-vanbandi {
  //background: url("~@/assets/images/card-vanbandi.png");
  //background-repeat: no-repeat;
  //background-size: cover;
  background: linear-gradient(135deg, #1b8b9a, #5ac7c7);
  color: #fff;
  min-height: 120px !important;
  height: 100%;

  & .mini-stat-label {
    color: #2a2a2a !important;
  }
}

.card-hopthu {
  //background: url("~@/assets/images/card-hopthu.png");
  //background-repeat: no-repeat;
  //background-size: cover;
  background: linear-gradient(135deg, #c04141, #e59191);
  color: #fff;
  min-height: 120px !important;
  height: 100%;
  & .mini-stat-label {
    color: #2a2a2a !important;
  }
}
.linkcolor:hover
   {
    background-color: rgb(218, 235, 250);
}
.card-thongbao {
  //background: url("~@/assets/images/card-thongbao.png");
  //background-repeat: no-repeat;
  //background-size: cover;
  background: linear-gradient(135deg, #c98e0c, #eab46f);
  color: #fff;
  min-height: 120px !important;
  height: 100%;

  & .mini-stat-label {
    color: #2a2a2a !important;
  }
}
</style>
