<script>
import Layout from "../../layouts/main";
import {required} from "vuelidate/lib/validators";
import appConfig from "@/app.config";
import {notifyModel} from "@/models/notifyModel";
import {pagingModel} from "@/models/pagingModel";
import {CONSTANTS} from "@/helpers/constants";
import Switches from "vue-switches";
import vue2Dropzone from "vue2-dropzone";
import {roomModel} from "@/models/roomModel";

export default {
  page: {
    title: "Phòng máy",
    meta: [{name: "description", content: appConfig.description}],
  },
  components: {Layout, Switches, vueDropzone: vue2Dropzone},
  data() {
    return {
      title: "Phòng máy",
      data: [],
      showModal: false,
      showDetail: false,
      showDeleteModal: false,
      submitted: false,
      model: roomModel.baseJson(),
      pagination: pagingModel.baseJson(),
      totalRows: 1,
      todoTotalRows: 1,
      currentPage: 1,
      numberOfElement: 1,
      perPage: 10,
      pageOptions: [5,10, 25, 50, 100],
      filter: null,
      filterOn: [],
      isBusy: false,
      sortBy: "age",
      sortDesc: false,
      fields: [
        { key: 'STT',
          label: 'STT',
          class: 'cs-text-center',
          sortable: false,
          thClass: 'hidden-sortable',
          thStyle: {width: '30px', minWidth: '30px'}
        },
        {
          key: "image",
          label: "Hình ảnh",
          class: 'cs-text-center',
          sortable: true,
          thStyle: {width: '100px', minWidth: '80px'}
        },
        {
          key: "name",
          label: "Tên",
          sortable: true,
        },
        {
          key: "isShow",
          label: "Hiển thị",
          class: 'td-xuly',
          sortable: true,
          thStyle: {width: '80px', minWidth: '80px'},
        },
        {
          key: "sort",
          label: "Thứ tự",
          class: 'td-xuly',
          sortable: true,
          thStyle: {width: '80px', minWidth: '80px'},
        },
        {
          key: 'process',
          label: 'Xử lý',
          class: 'td-xuly btn-process',
          thClass: 'hidden-sortable',
          sortable: false,
          thStyle: {width: '130px', minWidth: '130px'},
        }
      ],
      url :  process.env.VUE_APP_API_URL + 'files/view/',
      dropzoneOptions: {
        url: `${process.env.VUE_APP_API_URL}files/upload`,
        thumbnailWidth: 300,
        maxFilesize: 30,
        maxFiles:1,
        headers: { "My-Awesome-Header": "header value" },
        addRemoveLinks: true,
        acceptedFiles: ".jpeg,.jpg,.png,.gif"
      },
    };
  },
  validations: {
    model: {
      name: {required},
      address: {required},
      sort: {required}
    },
  },
  created() {
    this.fnGetList();
  },
  watch: {
    showModal(status) {
      if (status == false) this.model = roomModel.baseJson();
    },
    showDeleteModal(val) {
      if (val == false) {
        this.model.id = null;
      }
    },
  },
  methods: {
    handleSearch() {
      this.fnGetList()
    },
    fnGetList() {
      this.$refs.tblList?.refresh()
    },
    async handleUpdate(id) {
      await this.$store.dispatch("roomStore/getById", id).then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.model = res.data
          this.showModal = true;
        } else {
          this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
          this.fnGetList();
        }
      });
    },
    async handleDetail(id) {
      await this.$store.dispatch("roomStore/getById", id).then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.model = res.data
          this.showDetail = true;
        } else {
          this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
        }
      });
    },
    async handleDelete() {
      if (this.model.id != 0 && this.model.id != null && this.model.id) {
        await this.$store.dispatch("roomStore/delete", this.model.id).then((res) => {
          if (res.resultCode === 'SUCCESS') {
            this.fnGetList();
            this.showDeleteModal = false;
          }
          this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
          // });
        });
      }
    },
    handleShowDeleteModal(id) {
      this.model.id = id;
      this.showDeleteModal = true;
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
          await this.$store.dispatch("roomStore/update", this.model).then((res) => {
            if (res.resultCode === 'SUCCESS') {
              this.fnGetList();
              this.showModal = false;
            }
            this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res))
          });
        } else {
          // Create model
          await this.$store.dispatch("roomStore/create", this.model).then((res) => {
            if (res.resultCode === 'SUCCESS') {
              this.fnGetList();
              this.showModal = false;
              this.model = roomModel.baseJson();
            }
            this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res))
          });
        }
        loader.hide();
      }
      this.submitted = false;
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
        let promise =  this.$store.dispatch("roomStore/getPagingParams", params)
        return promise.then(resp => {
          if(resp.resultCode == CONSTANTS.SUCCESS){
            let data = resp.data;
            this.totalRows = data.totalRows
            let items = data.data
            this.numberOfElement = items.length
            this.loading = false
            return items || []
          }else{
            return [];
          }
        })
      } finally {
        this.loading = false
      }
    },
    removeHinhAnh(file, error, xhr){
      this.model.image= null;
    },
    addHinhAnh(file, response){
      if(this.model){
        let fileSuccess = response.data;
        this.model.image = fileSuccess.id;
      }
    },
    clearSearch(){
      this.filter = null;
    }
  }
}
</script>
<template>
  <Layout>
    <div class="row">
      <div class="col-12">
        <div class="card mb-2">
          <div class="card-body">
            <div class="row">
              <div class="col-md-4 col-12 d-flex align-items-center">
                <h4 class="font-size-18 fw-bold text-dark">Phòng máy</h4>
              </div>
              <div class="col-md-8 col-12 text-end">
                <b-button v-b-toggle.collapseSearch variant="light"
                          class="btn w-md btn-primary-outline me-2" size="sm">
                  <i class="fas fa-caret-down align-middle me-2"></i>
                  Tìm kiếm
                </b-button>
                <b-button
                    variant="primary"
                    type="button"
                    class="btn w-md btn-primary"
                    @click="showModal = true"
                    size="sm"
                >
                  <i class="mdi mdi-plus me-1"></i> Thêm phòng máy
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
                      <input
                          v-model = "filter"
                          type="text"
                          class="form-control"
                          placeholder="Tìm kiếm ..."
                      />
                      <i class="bx bx-search-alt search-icon"></i>
                    </div>
                    <div class="flex-grow-0 ms-2">
                      <div class="d-flex justify-content-between align-items-center flex-wrap">
                        <div class="flex-grow-1 mt-xl-0 mt-2">
                          <b-button @click="handleSearch" variant="light"
                                    class="btn w-md btn-primary me-2" size="md">
                            <i class="fas fa-search align-middle me-2"></i>
                            Tìm kiếm
                          </b-button>
                        </div>
                        <div class="flex-grow-1 mt-xl-0 mt-2">
                          <b-button @click="clearSearch" variant="light"
                                    class="btn w-md btn-secondary me-2" size="md">
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
              <div class="col-sm-8">
                <div class="text-sm-end">
                  <b-modal
                      v-model="showModal"
                      title="Thông tin"
                      title-class="text-black font-18"
                      body-class="p-3"
                      hide-footer
                      centered
                      no-close-on-backdrop
                      size="lg"
                  >
                    <form @submit.prevent="handleSubmit"
                          ref="formContainer">
                      <div class="row">
                        <div class="col-12">
                          <div class="mb-3">
                            <label class="text-left">Tên</label>
                            <span style="color: red">&nbsp;*</span>
                            <input type="hidden" v-model="model.id"/>
                            <input
                                id="ten"
                                v-model.trim="model.name"
                                type="text"
                                class="form-control"
                                placeholder=""
                                :class="{
                                'is-invalid':
                                  submitted && $v.model.name.$error,
                              }"
                            />
                            <div
                                v-if="submitted && !$v.model.name.required"
                                class="invalid-feedback"
                            >
                              Tên không được để trống.
                            </div>
                          </div>
                        </div>
                        <div class="col-12">
                          <div class="mb-3">
                            <label class="text-left">Địa chỉ</label>
                            <span style="color: red">&nbsp;*</span>
                            <input
                                id="ten"
                                v-model.trim="model.address"
                                type="text"
                                class="form-control"
                                placeholder=""
                                :class="{
                                'is-invalid':
                                  submitted && $v.model.address.$error,
                              }"
                            />
                            <div
                                v-if="submitted && !$v.model.address.required"
                                class="invalid-feedback"
                            >
                              Địa chỉ không được để trống.
                            </div>
                          </div>
                        </div>
                        <div class="col-6">
                          <div class="mb-3">
                            <label class="text-left">Lat</label>
                            <span style="color: red">&nbsp;*</span>
                            <input
                                id="thuTu"
                                v-model="model.lat"
                                type="text"
                                class="form-control"
                                placeholder=""
                            />
                          </div>
                        </div>
                        <div class="col-6">
                          <div class="mb-3">
                            <label class="text-left">Long</label>
                            <span style="color: red">&nbsp;*</span>
                            <input
                                id="thuTu"
                                v-model="model.long"
                                type="text"
                                class="form-control"
                                placeholder=""
                            />
                          </div>
                        </div>
                        <div class="col-12">
                          <div class="mb-3">
                            <label class="text-left">Google Map</label>
                            <input
                                id="thuTu"
                                v-model="model.googleMap"
                                type="text"
                                class="form-control"
                                placeholder=""
                            />
                          </div>
                        </div>
                        <div class="col-6">
                          <div class="mb-3">
                            <label class="text-left">Hiển thị</label>
                            <br />
                            <switches class="mt-2 mb-0"  v-model="model.isShow" type-bold="false" color="success" ></switches>
                          </div>
                        </div>
                        <div class="col-6">
                          <div class="mb-3">
                            <label class="text-left">Nhập số thứ tự</label>
                            <span style="color: red">&nbsp;*</span>
                            <input
                                id="thuTu"
                                v-model="model.sort"
                                type="number"
                                min="0"
                                oninput="validity.valid||(value='');"
                                class="form-control"
                                placeholder=""
                                :class="{
                                'is-invalid':
                                  submitted && $v.model.sort.$error,
                              }"
                            />
                            <div
                                v-if="submitted && !$v.model.sort.required"
                                class="invalid-feedback"
                            >
                              Thứ tự không được để trống.
                            </div>
                          </div>
                        </div>
                        <div class="col-12">
                          <label class="text-left"> Hình ảnh </label>
                          <div class=" profile-user-wid mt-2">
                            <div v-if="model.image != null">
                              <b-img
                                  style="height: 200px ; width: 300px"
                                  :src=" url + `${model.image}`">
                              </b-img>
                            </div>
                            <div v-else>
                              <img
                                  style="max-height: 280px ; width: 300px"
                                  src="@/assets/images/4.png"
                              />
                            </div>
                          </div>
                        </div>
                        <div class="col-12" >
                          <label class="text-left">Chọn ảnh </label>
                          <vue-dropzone
                              id="myVueDropzone"
                              ref="myVueDropzone"
                              :use-custom-slot="true"
                              :options="dropzoneOptions"
                              v-on:vdropzone-removed-file="removeHinhAnh"
                              v-on:vdropzone-success="addHinhAnh"
                          >
                            <div class="dropzone-custom-content">
                              <i class="display-1 text-muted bx bxs-cloud-upload " style="font-size: 70px"></i>
                              <h4>Kéo thả hình ảnh hoặc bấm vào để tải hình ảnh</h4>
                            </div>
                          </vue-dropzone>
                        </div>
                      </div>
                      <div class="text-end pt-2 mt-3">
                        <b-button variant="light" class="w-md" size="sm" @click="showModal = false">
                          Đóng
                        </b-button>
                        <b-button type="submit" variant="primary" size="sm" class="ms-1 w-md">
                          Lưu
                        </b-button>
                      </div>
                    </form>
                  </b-modal>
                </div>
              </div>
            </div>
            <div class="row">
              <div class="col-12">
                <div class="row mb-3">
                  <div class="col-sm-12 col-md-6">
                    <div id="tickets-table_length" class="dataTables_length">
                      <label class="d-inline-flex align-items-center">
                        Hiện
                        <b-form-select
                            class="form-select form-select-sm"
                            v-model="perPage"
                            size="sm"
                            :options="pageOptions"
                        ></b-form-select
                        >&nbsp;dòng
                      </label>
                    </div>
                  </div>
                </div>
                <div class="table-responsive-sm">
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
                      tbody-tr-class="b-table-chucvu"
                  >
                    <template v-slot:cell(STT)="data">
                      {{ data.index + ((currentPage-1)*perPage) + 1  }}
                    </template>
                    <template v-slot:cell(image)="data">
                      <div v-if="data.item.image != null">
                        <b-img
                            style="height: 100px ; width: 300px; object-fit: contain"
                            :src=" url + `${data.item.image}`">
                        </b-img>
                      </div>
                    </template>
                    <template v-slot:cell(isShow)="data">
                      <span v-if="data.item.isShow" class="badge bg-success ms-1">Hiển thị</span>
                      <span v-else class="badge bg-warning ms-1">Ẩn</span>
                    </template>
                    <template v-slot:cell(process)="data">
                      <button
                          type="button"
                          size="sm"
                          class="btn btn-edit btn-sm"
                          data-toggle="tooltip" data-placement="bottom" title="Cập nhật"
                          v-on:click="handleUpdate(data.item.id)">
                        <i class="fas fa-pencil-alt"></i>
                      </button>
                      <button
                          type="button"
                          size="sm"
                          class="btn btn-delete btn-sm"
                          data-toggle="tooltip" data-placement="bottom" title="Xóa"
                          v-on:click="handleShowDeleteModal(data.item.id)">
                        <i class="fas fa-trash-alt"></i>
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
                    <div>Hiển thị {{numberOfElement}} trên tổng số {{totalRows}} dòng</div>
                  </b-col>
                  <div class="col">
                    <div
                        class="dataTables_paginate paging_simple_numbers float-end">
                      <ul class="pagination pagination-rounded mb-0">
                        <b-pagination
                            v-model="currentPage"
                            :total-rows="totalRows"
                            :per-page="perPage"
                        ></b-pagination>
                      </ul>
                    </div>
                  </div>
                </div>
              </div>
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
            <b-button v-b-modal.modal-close_visit
                      size="sm"
                      class="btn btn-outline-info w-md"
                      v-on:click="showDeleteModal = false">
              Đóng
            </b-button>
            <b-button v-b-modal.modal-close_visit
                      size="sm"
                      variant="danger"
                      type="button"
                      class="w-md"
                      v-on:click="handleDelete">
              Xóa
            </b-button>
          </template>
        </b-modal>
      </div>
    </div>
  </Layout>
</template>
<style>
.td-stt {
  text-align: center;
}
.td-xuly {
  text-align: center;
}
.table>tbody> tr >td{
  padding: 0px;
  line-height: 30px;
}
.hidden-sortable:after, .hidden-sortable:before {
  display: none !important;
}

@media only screen and (max-width: 768px) {
  .b-table-chucvu>td:nth-of-type(1):before{
    content: "STT";
    font-weight: bold;
    color: #00568c;
  }
  .b-table-chucvu>td:nth-of-type(2):before {
    content: "Tên";
    font-weight: bold;
    color: #00568c;
  }
  .b-table-chucvu>td:nth-of-type(3):before {
    content: "Thứ tự";
    font-weight: bold;
    color: #00568c;
  }
  .b-table-chucvu>td:nth-of-type(4):before {
    content: "";
    font-weight: bold;
    color: #00568c;
  }
}

</style>