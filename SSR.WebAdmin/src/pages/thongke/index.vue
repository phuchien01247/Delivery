<script>
import Layout from "../../layouts/main1";
import PageHeader from "@/components/page-header";
import { numeric, required } from "vuelidate/lib/validators";
import appConfig from "@/app.config";
import { notifyModel } from "@/models/notifyModel";
import { pagingModel } from "@/models/pagingModel";
import Multiselect from "vue-multiselect";
import Treeselect from "@riophae/vue-treeselect";
import { donViModel } from "@/models/donViModel";
import { yeucauloiModel } from "@/models/yeucauloiModel";
import { stepModel } from "@/models/stepModel";
import { CONSTANTS } from "@/helpers/constants";
import { thongkeModel } from "@/models/thongkeModel";
import {templateThongKe} from "@/models/templateThongke";
export default {
  page: {
    title: "Thống kê",
    meta: [{ name: "description", content: appConfig.description }],
  },
  components: {
    Layout,
    //Multiselect,
    Treeselect,
  },

  data() {
    return {
      title: "Thống kê",
      model: donViModel.baseJson(),


      data: [],
      exportFiles: [],
      donviList: [],
      fields: [
        {
          key: 'STT',
          label: 'STT',
          class: 'cs-text-center',
          sortable: false,
          thClass: 'hidden-sortable',
          thStyle: { width: '30px', minWidth: '30px' }
        },

        {
          key: "a",
          label: "Lỗi",
          class: 'td-xuly',
          sortable: true,
          thStyle: { width: '80px', minWidth: '80px' },
        }
      ],
      currentPage: 1,
      perPage: 10,
      pageOptions: [5, 10, 25, 50, 100],
      option: ["Tất cả", "Phòng ban", "Không"],
      showModal: false,
      showDeleteModal: false,
      submitted: false,
      sortBy: "age",
      filter: null,
      sortDesc: false,
      filterOn: [],
      numberOfElement: 1,
      totalRows: 1,
      pagination: pagingModel.baseJson(),
      itemFilter: {
        code: null,
        name: null,
      },
      treeView: [],
      render: null,
      templateThongKe: templateThongKe.PhongData,
    };
  },
  validations: {
    model: {
      name: { required },
      description: { required },
      color: { required },

    },
  },
  methods: {
    clearSearch() {
      this.itemFilter.code = null;
      this.itemFilter.name = null;
    },
    formatRemovedonVi(node, instanceId) {
      let value = this.model.donVi?.find(x => x == node.id);
      if (value != null) {
        this.model.donVi = this.model.donVi.filter(x => x != value);
      }
    },
    changeValueDonViDefault(){
      if(!this.model.donVi ){
        this.model.donVi = [];
      }
     this.templateThongKe.map(value =>{
       this.model.donVi.push(value.id);
     })
    },
    formatdonVi(node, instanceId) {
      let index = this.model.donVi?.findIndex(x => x == node.id);
      if (index == -1 || index == undefined) {
        if (!this.model.donVi) {
          this.model.donVi = [];
        }
        this.model.donVi.push(node.id);
      }
    },
    normalizer(node) {
      if (node.children == null || node.children == 'null') {
        delete node.children;
      }
    },
    async fnGetList() {
      this.$refs.tblList?.refresh()
    },
    async GetExport() {
      // let currentProjectLocal = localStorage.getItem('currentProject');
      // this.slug = JSON.parse(currentProjectLocal);
      //lấy ds yêu cầu
      await this.$store.dispatch("thongkeStore/get").then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.exportFiles = res.data;
        }
        else {
          return [];
        }
      })
      await this.$store.dispatch("donViStore/get").then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.donviList = res.data;
        }
        else {
          return [];
        }
      })
      await this.$store.dispatch("thongkeStore/getHeader").then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.headers = res.data;
        }
        else {
          return [];
        }
      })
    },

    async handleDelete() {
      if (this.model.id != 0 && this.model.id != null && this.model.id) {
        await this.$store.dispatch("stepStore/delete", this.model.id).then((res) => {
          if (res.resultCode === 'SUCCESS') {
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

    async handleUpdate(id) {
      await this.$store.dispatch("stepStore/getById", id).then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.model = stepModel.toJson(res.data);
          this.showModal = true;
        } else {
          this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
        }
      });
    },
    async GetList() {
      let currentProjectLocal = localStorage.getItem('currentProject');
      this.slug = JSON.parse(currentProjectLocal);
      //lấy ds yêu cầu
      await this.$store.dispatch("yeucauloiStore/get").then((res) => {
        this.listissue = res.data;
      })

      //lấy ds dự án
      this.$store.dispatch("projectStore/getBySlug", this.slug).then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.listProject = res.data;
          this.nameproject = JSON.parse(currentProjectLocal); //lưu tên dự án đang mở

          //tìm tên dự án đang mở trong listproject để lấy id
          if (this.listProject) {
            this.idproject = this.listProject.id;     //chứa idproject đang mở
          }
          //console.log(this.idproject);
          //tìm có idproject giống

          // this.$store.dispatch("thongkeStore/get", this.idproject).then((res) => {
          //   this.exportFiles = res.data;
          // })
          // this.$store.dispatch("thongkeStore/getHeader", this.model).then((res) => {
          //   this.headers = res.data;
          // })



        }
      });

    },
    myProvider(ctx) {
      const params = {
        start: ctx.currentPage,
        limit: ctx.perPage,
        content: this.filter,
        sortBy: ctx.sortBy,
        sortDesc: ctx.sortDesc,
      }
      this.loading = true
      try {
        let promise = this.$store.dispatch("yeucauloiStore/getPagingParams", params)
        return promise.then(resp => {
          if (resp.resultCode == CONSTANTS.SUCCESS) {
            let data = resp.data;
            this.totalRows = data.totalRows
            let items = data.data
            this.numberOfElement = items.length
            this.loading = false
            return items || []
          } else {
            return [];
          }
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
    async GetTreeList() {
      await this.$store.dispatch("donViStore/getTree").then((res) => {
        this.treeView = res.data;
      })
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
    async handleSubmit(e) {
      e.preventDefault();
      let currentProjectLocal = JSON.parse( localStorage.getItem('currentProject'));
      let model = {projectId: currentProjectLocal, donViIds: this.model == null? []: this.model.donVi};
      await this.$store.dispatch("thongkeStore/renderTable", model).then((res) => {
            if (res.resultCode === 'SUCCESS') {
              this.render = res.data;
            }
            this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res))
          });
    },
    async handleExport() {
      let currentProjectLocal = JSON.parse( localStorage.getItem('currentProject'));
      let model = {projectId: currentProjectLocal, donViIds: this.model == null ? []: this.model.donVi};
      console.log(model);
      await this.$store
          .dispatch("thongkeStore/exportThongKe", model)
          .then((res) => {
            const blob = new Blob(
                [this.base64ToArrayBuffer(res.fileContents)],
                { type: "application/xlsx" }
            );
            const link = document.createElement("a");
            link.href = window.URL.createObjectURL(blob);
            const fileName = res.fileDownloadName;
            link.download = fileName;
            link.click();
          })
          .catch((e) => {
            console.log(e);
          });
    },
  },
  async created() {
    this.GetTreeList();
    // this.fnGetList;
    // this.GetList();
    // this.GetDonVi();
    this.changeValueDonViDefault();

  },
  mounted() {


  },
  watch: {
    model: {
      deep: true,
      handler(val) {

      }
    },
    showModal(status) {
      if (status == false) this.model = stepModel.baseJson();
    },

    showDeleteModal(val) {
      if (val == false) {
        this.model.id = null;
      }
    }
  },
};
</script>

<template>
  <Layout>
    <!--    <PageHeader :title="title" :items="items"/>-->
    <div class="row">
      <div class="col-12">
        <div class="card mb-2">
          <div class="card-body">
            <div class="row">
              <div class="col-md-4 col-12 d-flex align-items-center">
                <h4 class="font-size-18 fw-bold cs-title-page">Bảng thống kê</h4>
              </div>
              <div class="col-md-8 col-12 text-end">
                <b-button v-b-toggle.collapseSearch variant="light" class="btn w-md btn-primary-outline me-2" size="sm">
                  <i class="fas fa-caret-down align-middle me-2"></i>
                  Tìm kiếm
                </b-button>
                <b-button type="button" variant="primary" class="w-md" size="sm" @click="handleExport">
                  <i class="mdi mdi-file-excel"></i> Xuất Excel
                </b-button>
              </div>

            </div>
            <b-collapse id="collapseSearch" class="mt-1">
              <div class="row">
                <div class="col-12">
                  <div class="d-flex justify-content-between align-items-end flex-wrap mb-2">
                    <!-- Nội dung -->
                    <div class="flex-grow-1 me-2">
                      <label>Nội dung</label>
                      <input v-model="filter" type="text" class="form-control" placeholder="Tìm kiếm ..."
                        @input="search()" />
                      <i class="bx bx-search-alt search-icon"></i>
                    </div>
                    <div class="flex-grow-0 ms-2">
                      <div class="d-flex justify-content-between align-items-center flex-wrap">
<!--                        <div class="flex-grow-1 mt-xl-0 mt-2">-->
<!--                          <b-button @click="handleSearch" variant="light" class="btn w-md btn-primary me-2" size="md">-->
<!--                            <i class="fas fa-search align-middle me-2"></i>-->
<!--                            Tìm kiếm-->
<!--                          </b-button>-->
<!--                        </div>-->
                        <div class="flex-grow-1 mt-xl-0 mt-2">
                          <b-button @click="clearSearch" variant="light" class="btn w-md btn-secondary me-2" size="md">
                            <i class="fas fa-redo-alt align-middle me-2"></i>
                            Làm mới
                          </b-button>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </b-collapse>
          </div>
        </div>
      </div>
    </div>
    <div class="row">
      <div class="col-12">
        <div class="card">
          <div class="card-body">
            <div class="row mb-2">
              <form @submit.prevent="handleSubmit" ref="formContainer">
                <div class="col-sm-3">


                </div>
                <div class="col-sm-6">
                  <label class="text-left">Chọn đơn vị</label>
                  <treeselect v-on:select="formatdonVi" v-on:deselect="formatRemovedonVi" :limit="5"
                              :multiple="true"    :flat="true" :searchable="true" :normalizer="normalizer"
                              value-format="object" :options="treeView" :value="templateThongKe" :show-count="true"
                              :default-expand-level="1" placeholder="Chọn đơn vị tham gia"
                  >
                    <label slot="option-label" slot-scope="{ node, shouldShowCount, count, labelClassName, countClassName }"
                           :class="labelClassName">
                      {{ node.label }}
                      <span v-if="shouldShowCount" :class="countClassName">({{ count }})</span>
                    </label>
                  </treeselect>
                </div>
                <div class="col-sm-3" style="margin-top: 25px;">
                  <b-button type="button" variant="success" class="w-md" size="sm" @click="handleSubmit">
                    <i class="mdi mdi-eye"></i> Xem
                  </b-button>
                </div>
              </form>
            </div>
            <div class="row">
              <div class="col-12">
                <div class="row mb-3">
                  <div class="col-sm-12 col-md-6">
                    <!-- <div id="tickets-table_length" class="dataTables_length">
                        <label class="d-inline-flex align-items-center">
                          Hiện
                          <b-form-select class="form-select form-select-sm" v-model="perPage" size="sm"
                            :options="pageOptions"></b-form-select>&nbsp;dòng
                        </label>
                      </div> -->
                  </div>
                </div>
                <div class="table-responsive mb-0">

                  <div>


                    <template>
                      <div class="table-wrapper">
                        <table class="table-bordered" v-if="render">
                          <thead>
                            <tr>
                              <th class="hidden-sortable" v-for="header in render.header" :key="header.idDV">
                                <div class="center">
                                  {{ header.nameDV }}
                                </div>
                              </th>
                            </tr>
                          </thead>
                          <tbody>
                            <tr v-for="exportFile in render.body" :key="exportFile.idLB">
                              <td style="min-width: 200px;">
                                {{ exportFile.nameLB }}
                              </td>
                              <td v-for="(value, index) in exportFile.values" :key="index" style="max-width: 1000px; min-width: 200px; height: 50px;">
                                <div class="center">
                                  {{ value }}
                                </div>
                              </td>
                            </tr>
                          </tbody>
                        </table>
                      </div>
                    </template>

                  </div>


<!--                  <template v-if="isBusy">-->
<!--                    <div align="center">Đang tải dữ liệu</div>-->
<!--                  </template>-->
<!--                  <template v-if="totalRows <= 0 && !isBusy">-->
<!--                    <div align="center">Không có dữ liệu</div>-->
<!--                  </template>-->
                </div>


              </div>
            </div>
            <b-modal v-model="showDeleteModal" centered title="Xóa dữ liệu" title-class="font-18" no-close-on-backdrop>
              <p>
                Dữ liệu xóa sẽ không được phục hồi!
              </p>
              <template #modal-footer>
                <button v-b-modal.modal-close_visit class="btn btn-outline-info m-1" v-on:click="showDeleteModal = false">
                  Đóng
                </button>
                <button v-b-modal.modal-close_visit class="btn btn-danger btn m-1" v-on:click="handleDelete">
                  Xóa
                </button>
              </template>
            </b-modal>
          </div>
        </div>
      </div>
    </div>
  </Layout>
</template>
<style>
.td-stt {
  text-align: center;
  width: 55px;
}

.td-username {
  text-align: center;
  width: 120px;
}

.td-ten {
  text-align: center;
  width: 140px;
}

.td-email {
  text-align: center;
  width: 120px;
}

.td-donVi {
  text-align: center;
  width: 180px;
}

.td-xuly {
  text-align: center;
  width: 80px;
}

.td-sodienthoai {
  text-align: center;
  width: 100px;
}

.center {
  display: flex;
  justify-content: center;
  align-items: center;
}

.table {
  overflow: auto;
}

thead th {

  top: 0;
  left: 0;
  max-width: 200px;
  min-width: 100px;
}
</style>
