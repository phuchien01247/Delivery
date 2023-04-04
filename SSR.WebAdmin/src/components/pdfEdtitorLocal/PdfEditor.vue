<script>
import {readAsImage, readAsPDF, readAsDataURL} from './utils/asyncReader'
import prepareAssets, {fetchFont} from './utils/prepareAssets'
import {save} from './utils/PDF'
import {generateId} from './utils/helper'
import PdfPage from "@/components/pdfEditor/PdfPage";
import ObjectContainer from "@/components/pdfEditor/ObjectContainer";
import {CURRENT_USER} from "@/helpers/currentUser";
import {vanBanDenModel} from "@/models/vanBanDenModel";
import {notifyModel} from "@/models/notifyModel";
import {signDigitalModel} from "@/models/signDigitalModel";
import {mapState} from "vuex";
import {required} from "vuelidate/lib/validators";
import LetterCube from "@/components/LetterCube";
import moment from "moment";
import Countdown from "@/components/Countdown/countdown";

export default {
  // eslint-disable-next-line vue/no-unused-components
  components: {Countdown, ObjectContainer, PdfPage, LetterCube},
  props: {
    pdf: {
      required: false,
      type: String,
    },
    file: {required: false},
    fileInfo: {required: true}
  },
  validations: {
    modelTextSign: {
      content: {required},
    },
  },
  data() {
    return {
      pages: [],
      pagesSize: [],
      allObjects: [],
      pagesScale: [],
      selectedPageIndex: 0,
      signatureCanvas: {
        isShow: false
      },
      opacity: 1,
      pdfFile: null,
      kySoModel: {
        userName: null,
        password: null,
        file: this.file,
        px: 0,
        py: 0,
        width: 0,
        height: 0,
        page: 0,
        image: null
      },
      test: null,
      currentUser: CURRENT_USER.USER_KY_SO,
      currentUserKySo: CURRENT_USER.USER_KY_SO,
      downloadFile: null,
      currentFont: "Times-Roman",
      signatureDigital: {
        vanBanDiId: null,
        signDigitals: [],
        scale: 1
      },
      model: vanBanDenModel.baseJson(),
      currentUserName: CURRENT_USER.USERNAME,
      showThemChuKyModal: false,
      imageChuKyMoc: null,
      lists: [
        {
          img: 'https://avatars2.githubusercontent.com/u/15681693?s=460&v=4'
        }
      ],
      option: {
        img: 'https://avatars2.githubusercontent.com/u/15681693?s=460&v=4',
        size: 1,
        full: true,
        outputType: 'png',
        canMove: true,
        fixedBox: false,
        original: false,
        canMoveBox: true,
        autoCrop: true,
        autoCropWidth: 264,
        autoCropHeight: 100,
        centerBox: false,
        high: true,
        max: 99999
      },
      fixed: true,
      fixedNumber: [16, 9],
      previews: null,
      signatureSave: null,
      signatureVM: {
        userName: CURRENT_USER.USERNAME,
        action: null,
        signatureSaves: []
      },
      showModelNote: false,
      typeKySo: "KYNHAY",
      modelTextSign: {
        fontWeight: "normal",
        fontSize: 13,
        content: ""
      },
      submitted: false,
      resultData: {
        resultString: null,
        resultCode: null
      },
      isShowModalAfterThietLap: false,
      showModalHoanhThanhKySo: false,
      end: new Date(),
      trangThaiKySo: "Chờ xác nhận",
      trangThaiKySoCode: null,
    }
  },
  computed: {
    checkObjectExited() {
      console.log("allObject", this.allObjects)
      if (this.allObjects && this.allObjects.length > 0) {
        var indexAbsolute = this.allObjects.findIndex(x => x.absolute == 'undefined' || x.absolute == null || x.absolute == false);
        console.log( indexAbsolute)
        if(indexAbsolute != -1){
          return true;
        }
        return false;
      }
      return false;
    },
    danhSachKyNhay() {
      if (this.signatureVM && this.signatureVM.signatureSaves) {
        return this.signatureVM.signatureSaves.filter((value) => value.typeKySo == "COMMENT");
      }
      return [];
    },
    danhSachKySo() {
      if (this.signatureVM && this.signatureVM.signatureSaves) {
        return this.signatureVM.signatureSaves.filter((value) => value.typeKySo == "SIGN");
      }
      return [];
    },
    countPages() {
      if (this.pages)
        return this.pages.length;
      else return 0;
    },
    activePage() {
      return this.selectedPageIndex + 1
    },
    isShow() {
      if (this.$refs.signatureCanvas) {
        return this.$refs.signatureCanvas.isShow;
      }
      return false;
    },
    ...mapState('pdfEditorStore', ['scale','isFinish'])
  },
  watch: {
    selectedPageIndex(value) {
    },
    allObjects: {
      handler: function (value, oldVal) {
        if (value) {
          if (this.signatureDigital) {
            value.map(object => {
              var objectPage = this.pagesSize.find(x => x.page == object.page);
              if (objectPage != null && objectPage.widthPage == 0 && objectPage.heightPage == 0) {
                object.widthPage = Math.round(objectPage.widthPage);
                object.heightPage = Math.round(objectPage.heightPage);
                return object;
              }
            })
          }
        }
        this.signatureDigital.signDigitals = [];
        if (value) {
          if (this.signatureDigital) {
            value.map(object => {
              this.signatureDigital.signDigitals.push(object);
            })
          }
        }
      },
      deep: true
    }
  },
  async mounted() {
    this.signatureDigital.vanBanDiId = this.fileInfo.vanBanDiId;

    console.log("abc", this.fileInfo.vanBanDiId)
    this.mountPdf()
    this.handleGetVBD(this.fileInfo.vanBanDiId);
    this.getSignature();
  },
  methods: {
    handleCloseHoanThanhKySoModal(){
      this.$router.push("/xu-ly-van-ban-di");
    },
    endDate(){
      const time = moment.duration("00:05:00");
      const date = moment();
      date.add(time);
      this.end = date;
    },
    handleHideModal() {
      this.isShowModalAfterThietLap = false;
      this.$router.push('/xu-ly-van-ban-di')
    },
    async handleGetVBD(id) {
      let loader = this.$loading.show({
        container: this.$refs.formContainer,
      });
      await this.$store.dispatch("vanBanDiStore/getById", id).then((res) => {
        if (res.resultCode == "SUCCESS") {
          if (res.data) {
            this.model = res.data;
            if (this.model.signDigitals) {
              this.allObjects = signDigitalModel.toListModel(this.model.signDigitals)
              console.log("object", this.allObjects)
              this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage({
                resultString: "Tải chữ ký thành công",
                resultCode: "SUCCESS"
              }));
            }
          }
        }
        loader.hide();
      });
    },
    async handleSubmit(e) {
      this.handleClearResponse();
      let loader = this.$loading.show({
        container: this.$refs.formContainer,
      },{
        default: this.$createElement(LetterCube),
      });
      this.kySoModel.userName = this.currentUser.userNameKySo;
      this.kySoModel.password = this.currentUser.passwordKySo;
      await this.$store.dispatch("signDigitalStore/thietLapKySoPhapLy", this.signatureDigital).then((res) => {
        if (res.resultCode == 'SUCCESS') {
          this.resultData.resultCode = res.resultCode;
          this.resultData.resultString = "Thiết lập ký số thành công!";
          this.isShowModalAfterThietLap = true;
          // this.$emit('closeModel');
        }else{
          this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
        }

      });
      loader.hide()
    },
    handleClearResponse(){
      this.trangThaiKySoCode = null;
      this.trangThaiKySo = null;
    },
    async handleSubmitKySoPhapLy(e) {
      this.handleClearResponse();
      this.showModalHoanhThanhKySo = true;
      this.endDate();
      this.trangThaiKySo ="Chờ xác nhận";
      this.$store.dispatch("pdfEditorStore/setFinish", false);


      this.signatureDigital.scale = Number(this.scale);
      this.signatureDigital.userName = this.currentUserKySo.userNameKySo;
      this.signatureDigital.password = this.currentUserKySo.passwordKySo;

      await this.$store.dispatch("signDigitalStore/thucHienKySoPhapLy", this.signatureDigital).then((res) => {
        if (res.resultCode == 'SUCCESS') {
          // this.showModalHoanhThanhKySo = false;
          this.downloadFile = res.data.content;
          this.trangThaiKySo = res.resultString;
          this.$store.dispatch("pdfEditorStore/setFinish", true);
          this.trangThaiKySoCode = res.resultCode
        }else{
          // this.showModalHoanhThanhKySo = false;
          this.downloadFile = null;
          this.trangThaiKySo = res.resultString;
          this.$store.dispatch("pdfEditorStore/setFinish", true);
          this.trangThaiKySoCode = res.resultCode
        }
        this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
      });
    },
    async handleSubmitDongMocThemSo(e) {
      this.handleClearResponse();
      this.showModalHoanhThanhKySo = true;
      this.endDate();
      this.trangThaiKySo ="Chờ xác nhận";
      this.$store.dispatch("pdfEditorStore/setFinish", false);


      this.signatureDigital.scale = Number(this.scale);
      this.signatureDigital.userName = this.currentUserKySo.userNameKySo;
      this.signatureDigital.password = this.currentUserKySo.passwordKySo;

      await this.$store.dispatch("signDigitalStore/thucHienDongMocThemSo", this.signatureDigital).then((res) => {
        if (res.resultCode == 'SUCCESS') {
          // this.showModalHoanhThanhKySo = false;
          this.downloadFile = res.data.content;
          this.trangThaiKySo = res.resultString;
          this.$store.dispatch("pdfEditorStore/setFinish", true);
          this.trangThaiKySoCode = res.resultCode
        }else{
          // this.showModalHoanhThanhKySo = false;
          this.downloadFile = null;
          this.trangThaiKySo = res.resultString;
          this.$store.dispatch("pdfEditorStore/setFinish", true);
          this.trangThaiKySoCode = res.resultCode
        }
        this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
      });

    },
    async mountPdf() {
      try {
        const res = await fetch(this.fileInfo.path)
        console.log("tải file", res)
        const pdfBlob = await res.blob()
        await this.addPDF(pdfBlob)

        this.selectedPageIndex = 0

        setTimeout(() => {
          fetchFont('Times-Roman')
          prepareAssets()
        }, 5000)
      } catch (e) {
        console.log('Error:', e)
      }
    },

    async addPDF(file) {
      try {
        const pdf = await readAsPDF(file)
        this.pdfFile = file
        const numPages = pdf.numPages
        if (this.pages) {
          this.pages.splice(0, this.pages.length)
        }
        this.pages = Array(numPages).fill().map((_, i) => pdf.getPage(i + 1))

        this.pagesScale = Array(numPages).fill(1)
      } catch (e) {
        console.log('Failed to add pdf. Please try again.', e)
      }
    },

    onMeasure(scale, pageIndex) {
      this.pagesScale[pageIndex] = scale
    },

    selectPage(index) {
      this.selectedPageIndex = index;
    },

    updateObject(objectId, payload) {
      this.allObjects = this.allObjects.map((object) => {
        if (object.page == this.selectedPageIndex && object.id === objectId) {
          return {...object, ...payload}
        } else {
          return object
        }
      })
    },

    deleteObject(objectId) {
      this.allObjects = this.allObjects.filter((object) => object.id !== objectId)
      console.log(this.allObjects);
    },

    async addImage(file, type) {
      console.log("type", type)
      try {
        // get dataURL to prevent canvas from tainted
        const url = await readAsDataURL(file)
        const img = await readAsImage(url)
        const id = generateId();
        const {width, height} = img

        const object = {
          id,
          type: 'image',
          typeKySo: type,
          width,
          height,
          x: 0,
          y: 0,
          payload: img,
          imageBase64: img.currentSrc,
          file: file,
          page: this.selectedPageIndex,
        }
        var objectPage = this.pagesSize.find(x => x.page == object.page);
        if (objectPage != null) {
          object.widthPage = Math.round(objectPage.widthPage);
          object.heightPage = Math.round(objectPage.heightPage);
        }

        let indexImage = this.allObjects.findIndex(x => x.type == 'image');
        if (indexImage == -1) {
          this.allObjects = [...this.allObjects, object];
        } else {
          this.allObjects = this.allObjects.map(value => {
            if (value.typeKySo == 'SIGN') {
              value.payload = object.payload;
              value.imageBase64 = object.imageBase64;
              value.file = object.file
            }
            return value;
          });
          this.allObjects = [...this.allObjects, object];
          console.log(this.allObjects)
        }

      } catch (e) {
        console.log('Failed to add image.', e)
      }
    },

    async download() {
      try {
        await save(this.pdfFile, this.allObjects, 'PDF Copy', this.pagesScale)
      } catch (e) {
        console.log('Error on saving, please try again.', e)
      }
    },
    uploadMocChuKy(e) {
      const file = e.target.files[0]
      if (file && this.selectedPageIndex >= 0) {
        this.addImage(file, "COMMENT")
      }
      e.target.value = null
    },
    uploadImage(e) {
      const file = e.target.files[0]
      if (file && this.selectedPageIndex >= 0) {
        this.addImage(file, "SIGN")
      }
      e.target.value = null
    },

    downloadPDF() {
      const linkSource = `data:application/pdf;base64,${this.downloadFile}`;
      const downloadLink = document.createElement("a");
      const fileName = "abc.pdf";
      downloadLink.href = linkSource;
      downloadLink.download = fileName;
      downloadLink.click();
    },
    onAddTextField() {
      // if (this.selectedPageIndex >= 0) {
      //   this.addTextField();
      // }
      this.showModelNote = true
      this.modelTextSign = {
        fontWeight: "normal",
        fontSize: 13,
        content: ""
      }
    },
    addTextField(text = "Hãy nhập dữ liệu ...") {
      const id = generateId();
      fetchFont(this.currentFont);
      const object = {
        id,
        text,
        type: "text",
        typeKySo: "COMMENT",
        size: 16,
        width: 0, // recalculate after editing
        lineHeight: 1.4,
        fontFamily: this.currentFont,
        x: 0,
        y: 0,
        page: this.selectedPageIndex,
      };
      var objectPage = this.pagesSize.find(x => x.page == object.page);
      if (objectPage != null) {
        object.widthPage = Math.round(objectPage.widthPage);
        object.heightPage = Math.round(objectPage.heightPage);
      }
      this.allObjects = [...this.allObjects, object];
    },
    onUpdateSizePage(payload, indexPage) {
      if (this.pagesSize == null) {
        this.pagesSize = []
      }
      var index = this.pagesSize.findIndex(x => x.page == indexPage);
      if (index == -1) {
        this.pagesSize.push({page: indexPage, heightPage: payload.height, widthPage: payload.width});
      } else {
        this.pagesSize[index].heightPage = Math.round(payload.height);
        this.pagesSize[index].widthPage = Math.round(payload.width);
      }
    },


    handleRemoveImageSignature(id) {
      if (this.signatureVM && this.signatureVM.signatureSaves) {
        this.signatureVM.signatureSaves = this.signatureVM.signatureSaves.filter(x => x.id !== id);
        this.updateSignature("DELETE");
      }
    },
    addTextSignature() {
      this.submitted = true;
      this.$v.$touch();
      if (this.$v.modelTextSign.$invalid) {
        return;
      }
      const id = generateId();
      fetchFont(this.currentFont);
      const object = {
        id,
        fontWeight: this.modelTextSign.fontWeight,
        text: this.modelTextSign.content,
        type: "text",
        typeKySo: "COMMENT",
        size: this.modelTextSign.fontSize,
        width: 0, // recalculate after editing
        lineHeight: 1.4,
        fontFamily: this.currentFont,
        x: 0,
        y: 0,
        page: this.selectedPageIndex,
      };
      var objectPage = this.pagesSize.find(x => x.page == object.page);
      if (objectPage != null) {
        object.widthPage = Math.round(objectPage.widthPage);
        object.heightPage = Math.round(objectPage.heightPage);
      }
      this.allObjects = [...this.allObjects, object];
      this.showModelNote = false;
      console.log("object", object)
    },
    chonseImageSign(value) {

      value.page = this.selectedPageIndex;
      value.x = 0;
      value.y = 0;
      value.id = generateId();

      const object = value;
      var objectPage = this.pagesSize.find(x => x.page == object.page);
      if (objectPage != null) {
        object.widthPage = Math.round(objectPage.widthPage);
        object.heightPage = Math.round(objectPage.heightPage);
      }

      let indexImage = this.allObjects.findIndex(x => x.type == 'image');
      if (indexImage == -1) {
        this.allObjects = [...this.allObjects, object];
      } else {
        this.allObjects = this.allObjects.map(value => {
          if (value.typeKySo == 'SIGN') {
            value.payload = object.payload;
            value.imageBase64 = object.imageBase64;
            value.file = object.file
          }
          return value;
        });
        this.allObjects = [...this.allObjects, object];
        console.log(this.allObjects)
      }
      this.$refs.dropdownChuKyMoc.hide()
    },
    async addImageToPDF(type) {
      if (this.previews && this.selectedPageIndex >= 0) {
        this.$refs.cropper.getCropData((data) => {
          try {
            // get dataURL to prevent canvas from tainted
            // const img =  readAsImage(this.previews.url)
            const img = new Image();

            img.src = data;
            let width = this.previews.w;
            let height = this.previews.h;

            const id = generateId();
            // const width = this.previews.w;
            // const height = this.previews.h;
            // console.log("type",width, height)
            let tks = "SIGN";
            if (this.typeKySo == "KYNHAY")
              tks = "COMMENT";
            const object = {
              id,
              type: 'image',
              typeKySo: tks,
              width,
              height,
              x: 0,
              y: 0,
              imageBase64: data,
              page: this.selectedPageIndex,
            }
            console.log("object", object, this.typeKySo)
            var objectPage = this.pagesSize.find(x => x.page == object.page);
            if (objectPage != null) {
              object.widthPage = Math.round(objectPage.widthPage);
              object.heightPage = Math.round(objectPage.heightPage);
            }

            let indexImage = this.allObjects.findIndex(x => x.type == 'image');
            if (indexImage == -1) {
              this.allObjects = [...this.allObjects, object];
            } else {
              this.allObjects = this.allObjects.map(value => {
                if (value.typeKySo == 'SIGN') {
                  value.payload = object.payload;
                  value.imageBase64 = object.imageBase64;
                  value.file = object.file
                }
                return value;
              });
              this.allObjects = [...this.allObjects, object];
              console.log(this.allObjects)
            }

            this.showThemChuKyModal = false;

            if (this.signatureVM && !this.signatureVM.signatureSaves) {
              this.signatureVM.signatureSaves = [];
            }

            // Check ký nháy hay chữ ký
            if (this.signatureVM && this.signatureVM.signatureSaves) {
              let dsKySo = this.signatureVM.signatureSaves.filter((value) => value.typeKySo == "SIGN");
              let dsKyNhay = this.signatureVM.signatureSaves.filter((value) => value.typeKySo == "COMMENT");
              if (object.typeKySo == "SIGN" && dsKySo.length >= 2) {
                dsKySo.shift()
              } else if (object.typeKySo == "COMMENT" && dsKyNhay >= 2) {
                dsKyNhay.shift();
              }
              this.signatureVM.signatureSaves = dsKySo.concat(dsKyNhay);
            }
            let sign = {
              id: object.id, imageBase64: object.imageBase64, action: 1, type: 'image',
              typeKySo: object.typeKySo, width:
              width,
              height: height,
            }
            this.signatureVM.signatureSaves.push(sign);
            this.updateSignature("ADD");
            this.$refs.dropdownChuKyMoc.hide()
          } catch (e) {
            console.log('Failed to add image.', e)
          }
        })
      }
    },
    handleZoomIn() {
      if (this.scale >= 2) {
        return;
      }
      const _scale = (parseFloat(Number(JSON.parse(JSON.stringify(this.scale))) + 0.2).toFixed(1));
      this.$store.dispatch("pdfEditorStore/setScale", _scale);
    },
    handleZoomOut() {
      if (this.scale <= 0.4) {
        return;
      }
      const _scale = (parseFloat(Number(JSON.parse(JSON.stringify(this.scale))) - 0.2).toFixed(1));
      this.$store.dispatch("pdfEditorStore/setScale", _scale);
    },
    nextPage() {
      if (this.selectedPageIndex >= this.pages.length - 1)
        return;
      this.selectedPageIndex += 1;
      let element = document.getElementById('page-signature-' + this.selectedPageIndex);
      element.scrollIntoView({behavior: "smooth"});
    },
    previousPage() {
      if (this.selectedPageIndex <= 0)
        return;
      this.selectedPageIndex -= 1;
      let element = document.getElementById('page-signature-' + this.selectedPageIndex);
      element.scrollIntoView({behavior: "smooth"});
    },
    async uploadChuKyMoc(e) {
      const file = e.target.files[0]
      if (file) {
        const url = await readAsDataURL(file)
        this.option.img = url;
        this.showThemChuKyModal = true;
        this.typeKySo = "CHUKYMOC"
      }
      e.target.value = null
    },
    async uploadChuKyNhay(e) {
      this.typeKySo = "KYNHAY"
      console.log("this.typeKySo", this.typeKySo)
      const file = e.target.files[0]
      if (file) {
        const url = await readAsDataURL(file)
        this.option.img = url;
        this.showThemChuKyModal = true;

      }
      e.target.value = null
    },
    async updateSignature(action) {
      this.signatureVM.action = action;
      await this.$store.dispatch("userStore/updateSignature", this.signatureVM).then((res) => {
        if (res.resultCode == 'SUCCESS') {
          console.log(res)
        }
        this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
      });
    },
    async getSignature() {
      await this.$store.dispatch("userStore/getSignature", CURRENT_USER.USERNAME).then((res) => {
        if (res.resultCode == 'SUCCESS') {
          this.signatureVM.signatureSaves = res.data;
        }
      });
    },
    realTime(data) {
      this.previews = data
      console.log(data)
    },
    imgLoad(msg) {
      console.log(msg)
    },
    cropMoving(data) {
      console.log(data, '截图框当前坐标')
    },
  }
}


</script>
<template>
  <div class="tw-mb-4 tw-flex tw-flex-col tw-relative kyso-box" ref="formContainer">
    <b-card class="card-kyso">
      <div class="nav-kyso">
        <div class="d-flex row nav-kyso-box">
          <!--nav left-->
          <div class="d-flex col-lg-6 col-md-6 col-sm-12 content-nav-left">
            <div class="nav-kyso-left">
              <!--Số trang-->
              <div class="pdf-pages">
                <div class="icon-left px-2" @click="previousPage">
                  <i class="fas fa-arrow-left text-white"></i>
                </div>
                <div class="number-pages text-primary d-flex align-items-center">
                  <button class="btn btn-next text-primary">
                    {{ activePage }}
                  </button>
                  <div class="text-white mx-1">/</div>
                  <button class="btn btn-prev text-white">
                    {{ countPages }}
                  </button>
                </div>
                <div class="icon-left px-2" @click="nextPage">
                  <i class="fas fa-arrow-right text-white"></i>
                </div>
              </div>
              <!--              <div class="nav-line">-->
              <!--              </div>-->
              <!--              <div class="pdf-zoom px-2">-->
              <!--                <div class="icon-zoom-left">-->
              <!--                  <button @click="handleZoomOut">-->
              <!--                    <i class="fas fa-search-minus fs-5 text-white"></i>-->
              <!--                  </button>-->
              <!--                </div>-->
              <!--                <div>-->
              <!--                  <button class="btn btn-number-zoom mx-3">{{ scale * 100 }}%</button>-->
              <!--                </div>-->
              <!--                <div class="icon-zoom-right">-->
              <!--                  <button @click="handleZoomIn">-->
              <!--                    <i class="fas fa-search-plus fs-5 text-white"></i>-->
              <!--                  </button>-->
              <!--                </div>-->
              <!--              </div>-->
            </div>
          </div>
          <!--Nav right-->
          <div class="nav-kyso-right d-flex flex-wrap col-lg-6 col-md-6 col-sm-12">
            <div class="btn-group ">
              <b-dropdown
                  id="dropdown"
                  right
                  class="my-2 me-1"
                  menu-class="dropdown-menu-end mb-4"
                  ref="dropdownChuKyMoc"
              >
                <template slot="button-content">
                  <div class="d-flex justify-content-center align-items-center fw-bold" style="color: #2a2a2a;">
                    <i class="" height="60">
                      <img src="~@/assets/images/icon-chu-ky.png" alt="" width="25px">
                    </i>
                    Chữ ký/Mộc
                    <i class="mdi mdi-chevron-down"></i>
                  </div>
                </template>
                <div style="padding: 0 20px">
                  <template v-if="danhSachKySo">
                    <div v-for="(value, index) in danhSachKySo" style="margin-bottom: 10px" :key="index">
                      <div><img class="chuky-box" style="object-fit: contain" :src="`${value.imageBase64}`"/>
                      </div>
                      <div class="d-flex mt-1">
                        <button
                            @click="chonseImageSign(value)"
                            class="btn btn-selected w-50"
                        >
                          Chọn
                        </button>
                        <button
                            @click="handleRemoveImageSignature(value.id)"
                            class="btn btn-delete w-50"
                        >
                          Xoá
                        </button>
                      </div>
                    </div>
                  </template>

                  <div class="btn-chuky" style="min-width: 250px">
                    <input type="file" id="uploadImage" name="uploadImage" class="tw-hidden"
                           accept="image/png, image/jpg, image/jpeg" @change="uploadChuKyMoc"/>
                    <label for="uploadImage"
                           class="tw-text-black tw-cursor-pointer tw-font-medium tw-text-sm tw-px-3 tw-py-1.5 tw-text-center tw-mr-2 tw-mb-2">
                      Thêm Chữ ký/Mộc
                    </label>
                  </div>
                </div>

              </b-dropdown>
            </div>
            <!--            <div class="btn-group ">-->
            <!--              <b-dropdown-->
            <!--                  id="dropdown"-->
            <!--                  right-->
            <!--                  class="my-2 me-1"-->
            <!--                  menu-class="dropdown-menu-end mb-4"-->
            <!--                  ref="dropdownChuKyMoc"-->
            <!--              >-->
            <!--                <template slot="button-content">-->
            <!--                  <div class="d-flex justify-content-center align-items-center fw-bold"  style="color: #2a2a2a;">-->
            <!--                    <i class="fas fa-signature" height="60">-->
            <!--                    </i>-->
            <!--                    Chữ ký nháy-->
            <!--                    <i class="mdi mdi-chevron-down"></i>-->
            <!--                  </div>-->
            <!--                </template>-->
            <!--                <div style="padding: 0 20px">-->
            <!--                  <template v-if="danhSachKyNhay">-->
            <!--                    <div v-for="(value, index) in danhSachKyNhay" style="margin-bottom: 10px" :key="index">-->
            <!--                      <div ><img class="chuky-box" style="object-fit: contain" :src="`${value.imageBase64}`"/>-->
            <!--                      </div>-->
            <!--                      <div class="d-flex mt-1">-->
            <!--                        <button-->
            <!--                            @click="chonseImageSign(value)"-->
            <!--                            class="btn btn-selected w-50"-->
            <!--                        >-->
            <!--                          Chọn-->
            <!--                        </button>-->
            <!--                        <button-->
            <!--                            @click="handleRemoveImageSignature(value.id)"-->
            <!--                            class="btn btn-delete w-50"-->
            <!--                        >-->
            <!--                          Xoá-->
            <!--                        </button>-->
            <!--                      </div>-->
            <!--                    </div>-->
            <!--                  </template>-->

            <!--                  <div class="btn-chuky" style="min-width: 250px">-->
            <!--                    <input type="file" id="uploadImage1" name="uploadImage1" class="tw-hidden" accept="image/png, image/jpg, image/jpeg" @change="uploadChuKyNhay"/>-->
            <!--                    <label for="uploadImage1"-->
            <!--                           class="tw-text-black tw-cursor-pointer tw-font-medium tw-text-sm tw-px-3 tw-py-1.5 tw-text-center tw-mr-2 tw-mb-2">-->
            <!--                      Thêm Chữ ký nháy-->
            <!--                    </label>-->
            <!--                  </div>-->
            <!--                </div>-->

            <!--              </b-dropdown>-->
            <!--            </div>-->
            <!--            <div class="btn-kynhay btn">-->
            <!--              <input type="file" id="image" name="image" class="tw-hidden" @change="uploadMocChuKy"/>-->
            <!--              <label for="image"-->
            <!--                     class="tw-text-black tw-cursor-pointer tw-font-medium tw-text-sm tw-px-3 tw-py-1.5 tw-text-center tw-mr-2 tw-mb-2">-->
            <!--                Ký nháy-->
            <!--              </label>-->
            <!--            </div>-->

            <button
                class="btn btn-sm btn-ghichu d-flex align-items-center hover:tw-bg-blue-800 focus:tw-ring-4 focus:tw-ring-blue-300 tw-px-3 tw-py-1.5 tw-font-medium tw-text-sm tw-text-center tw-mr-2 tw-mb-2 dark:tw-bg-blue-600 dark:hover:tw-bg-blue-700 dark:focus:tw-ring-blue-800"
                @click="onAddTextField"
            >
              <img src="~@/assets/images/a.b.svg" class="text-white me-1" alt="icon ab" style="height: 12px !important">
              Nội dung
            </button>
            <!--            <template v-if="model.trangThai">-->
            <!--              <button-->
            <!--                  v-if="model.trangThai.code == 'kpl' && model.ower && model.ower.userName == currentUserName"-->
            <!--                  class="btn-sm bg-warning btn-kysophaply fw-bold"-->
            <!--                  @click="handleSubmitKySoPhapLy"-->
            <!--                  style="width: 100px"-->
            <!--              >-->
            <!--                Đồng ý ký-->
            <!--              </button>-->
            <!--              <button-->
            <!--                  v-if="model.trangThai.code == 'tkhtxn' && model.ower && model.ower.userName == currentUserName"-->
            <!--                  class="btn-sm bg-warning btn-kysophaply fw-bold"-->
            <!--                  @click="handleSubmit"-->
            <!--              >-->
            <!--                Chấp nhận thiết lập-->
            <!--              </button>-->
            <!--              <button-->
            <!--                  v-if="model.trangThai.code == 'htks' && model.ower && model.ower.userName == currentUserName"-->
            <!--                  class="btn-sm bg-warning btn-kysophaply fw-bold"-->
            <!--                  @click="handleSubmitDongMocThemSo"-->
            <!--              >-->
            <!--                Đã hoàn thành-->
            <!--              </button>-->
            <!--            </template>-->

          </div>
        </div>
      </div>
      <!--      <div class="d-flex justify-content-center bg-body">-->
      <!--        <div class="nav-kyso-left">-->
      <!--          &lt;!&ndash;Số trang&ndash;&gt;-->
      <!--          <div class="pdf-pages">-->
      <!--            <div class="icon-left px-2" @click="previousPage">-->
      <!--              <i class="fas fa-arrow-left text-primary"></i>-->
      <!--            </div>-->
      <!--            <div class="number-pages text-primary">-->
      <!--              <button class="btn btn-next text-primary">-->
      <!--                {{ activePage }}-->
      <!--              </button>-->
      <!--              /-->
      <!--              <button class="btn btn-prev text-primary">-->
      <!--                {{ countPages }}-->
      <!--              </button>-->
      <!--            </div>-->
      <!--            <div class="icon-left px-2" @click="nextPage">-->
      <!--              <i class="fas fa-arrow-right text-primary"></i>-->
      <!--            </div>-->
      <!--          </div>-->
      <!--&lt;!&ndash;          <div class="nav-line">&ndash;&gt;-->
      <!--&lt;!&ndash;          </div>&ndash;&gt;-->
      <!--          &lt;!&ndash;Zoom&ndash;&gt;-->
      <!--&lt;!&ndash;          <div class="pdf-zoom px-2">&ndash;&gt;-->
      <!--&lt;!&ndash;            <div class="icon-zoom-left">&ndash;&gt;-->
      <!--&lt;!&ndash;              <button @click="handleZoomOut">&ndash;&gt;-->
      <!--&lt;!&ndash;                <i class="fas fa-search-minus fs-5 text-primary"></i>&ndash;&gt;-->
      <!--&lt;!&ndash;              </button>&ndash;&gt;-->
      <!--&lt;!&ndash;            </div>&ndash;&gt;-->
      <!--&lt;!&ndash;            <div>&ndash;&gt;-->
      <!--&lt;!&ndash;              <button class="btn btn-number-zoom mx-3">{{ scale * 100 }}%</button>&ndash;&gt;-->
      <!--&lt;!&ndash;            </div>&ndash;&gt;-->
      <!--&lt;!&ndash;            <div class="icon-zoom-right">&ndash;&gt;-->
      <!--&lt;!&ndash;              <button @click="handleZoomIn">&ndash;&gt;-->
      <!--&lt;!&ndash;                <i class="fas fa-search-plus fs-5 text-primary"></i>&ndash;&gt;-->
      <!--&lt;!&ndash;              </button>&ndash;&gt;-->
      <!--&lt;!&ndash;            </div>&ndash;&gt;-->
      <!--&lt;!&ndash;          </div>&ndash;&gt;-->
      <!--        </div>-->
      <!--      </div>-->
      <!--      file pdf -->
      <div class="tw-w-full tw-overflow-auto" style="overflow-y:auto; height: 80vh">
        <div
            v-for="(page, pageIndex) in pages"
            :key="pageIndex"
            class="tw-p-1 tw-w-full tw-flex tw-flex-col tw-items-center tw-overflow-hidden"
            @mousedown="() => selectPage(pageIndex)"
            @touchstart="() => selectPage(pageIndex)"
            :id="`page-signature-${pageIndex}`"
        >
          <div :class="['tw-relative tw-shadow-lg tw-mb-4', { 'selected-pdf': pageIndex == selectedPageIndex }]">
            <pdf-page @updateSizePage="(payload) => onUpdateSizePage(payload, pageIndex)" :page="pages[pageIndex]"
                      @measure="(payload) => onMeasure(payload, pageIndex)"/>
            <div

                class="tw-absolute tw-top-0 tw-left-0 tw-transform tw-origin-top-left"
                :style="{ transform: `scale(${pagesScale[pageIndex]})`, touchAction: 'none' }"
            >
              <div v-for="(object, objectIndex) in allObjects"
                   :key="objectIndex">

                <template v-if="object.page == pageIndex">
                  <template v-if="object && object.type == 'image'">
                    <object-container
                        @update="(payload) => updateObject(object.id, payload)"
                        @delete="() => deleteObject(object.id)"
                        :file="object.file"
                        :payload="object.payload"
                        :imageBase64="object.imageBase64"
                        :x="Number(object.x)"
                        :y="Number(object.y)"
                        :width="object.width"
                        :height="object.height"
                        :opacity="opacity"
                        :pageScale="pagesScale[pageIndex]"
                        :type="object.type"
                        :path="object.type"
                        :object="object"
                    />
                  </template>

                  <template v-else-if="object && object.type == 'text'">
                    <object-container

                        @update="(payload) => updateObject(object.id, payload)"
                        @delete="() => deleteObject(object.id)"
                        :payload="object.payload"
                        :x="Number(object.x)"
                        :y="Number(object.y)"
                        :width="object.width"
                        :height="object.height"
                        :opacity="opacity"
                        :pageScale="pagesScale[pageIndex]"
                        :type="object.type"
                        :path="object.type"
                        :object="object"
                    />
                  </template>

                </template>
              </div>
            </div>
          </div>
        </div>
      </div>
    </b-card>
    <div class="footer-kyso d-flex justify-content-end">
      <!--      <button class="btn btn-outline-light fw-semibold btn-light text-dark px-4 me-2">Huỷ</button>-->
<!--      <button-->
<!--          class="btn text-primary px-4 fw-semibold me-2"-->
<!--          style="background-color: rgba(12,92,168, 0.1)"-->
<!--          @click="handleSubmitKySoPhapLy()"-->
<!--          :disabled="!checkObjectExited"-->
<!--      >Hoàn thành & Ký số-->
<!--      </button>-->
                    <button
                        v-if="model.trangThai && model.trangThai.code == 'tkhtxn' && model.ower && model.ower.userName == currentUserName"
                        class="btn text-primary px-4 fw-semibold me-2"
                        @click="handleSubmit"
                        style="background-color: rgba(12, 92, 168, 0.1)"
                        :disabled="!checkObjectExited"
                    >
                      Chấp nhận thiết lập
                    </button>
                    <button
                        v-if="model.trangThai && model.trangThai.code == 'kpl' && model.ower && model.ower.userName == currentUserName"
                        class="btn text-primary px-5 fw-semibold me-2"
                        @click="handleSubmitKySoPhapLy"
                        style="background-color: rgba(12, 92, 168, 0.1)"
                        :disabled="!checkObjectExited"
                    >
                      Đồng ý ký
                    </button>
                    <button
                        v-if="model.trangThai && model.trangThai.code == 'htks' && model.ower && model.ower.userName == currentUserName"
                        class="btn text-primary px-5 fw-semibold me-2"
                        @click="handleSubmitDongMocThemSo"
                        style="background-color: rgba(12, 92, 168, 0.1)"
                        :disabled="!checkObjectExited"
                    >
                      Đã hoàn thành
                    </button>
    </div>

    <!-- Thêm chữ ký -->
    <b-modal
        v-model="showThemChuKyModal"
        title-class="font-18 text-primary fw-bold"
        footer-class="bg-secondary py-1"
        no-close-on-backdrop
        size="md"
    >
      <template #modal-title>
        <div class="text-primary fw-bold text-center">
          <h4>Thêm chữ ký/Mộc</h4>
        </div>
      </template>
      <div style="height: 200px; width: 100%">
        <vue-cropper ref="cropper" :img="option.img" :output-size="option.size" :output-type="option.outputType"
                     :info="true" :full="option.full" :fixed="fixed" :fixed-number="fixedNumber"
                     :can-move="option.canMove" :can-move-box="option.canMoveBox" :fixed-box="option.fixedBox"
                     :original="option.original"
                     :auto-crop="option.autoCrop" :auto-crop-width="option.autoCropWidth"
                     :auto-crop-height="option.autoCropHeight" :center-box="option.centerBox"
                     @real-time="realTime" :high="option.high"
                     @img-load="imgLoad" mode="cover" :max-img-size="option.max"
                     @crop-moving="cropMoving"></vue-cropper>
      </div>
      <!--      <button @click="down">Download</button>-->
      <template #modal-footer="{cancel}">
        <b-button class="btn bg-white me-2 text-primary px-5" @click="cancel()">Huỷ</b-button>
        <button
            @click="addImageToPDF"
            class="btn px-5 text-white"
            style="background-color: #3082c9;"
        >
          Lưu
        </button>
      </template>

    </b-modal>
    <!--  Modal thêm ghi chú -->
    <b-modal
        v-model="showModelNote"
        body-class="p-3"
        headerClass="justify-content-center"
        footerClass="bg-light py-1"
        centered
        no-close-on-backdrop
        size="md"
    >
      <template #modal-title>
        <div class="text-primary fw-bold text-center">
          <h4>Nhập nội dung</h4>
        </div>
      </template>
      <form action="">
        <div class="row">
          <div class="col-6 mb-2">
            <div>
              <label for="">Kiểu chữ</label>
              <select v-model="modelTextSign.fontWeight" class="form-select">
                <option selected value="normal">Normal</option>
                <option value="bold">Bold</option>
              </select>
            </div>
          </div>
          <div class="col-6 mb-2">
            <div>
              <label for="">Kích thước</label>
              <input
                  v-model="modelTextSign.fontSize"
                  type="number"
                  min="8"
                  max="30"
                  class="form-control"
              />
            </div>
          </div>
          <div class="col-12 mb-2">
            <div>
              <label for="">Nội dung hiển thị</label>
              <textarea
                  v-model="modelTextSign.content"
                  type="text"
                  class="form-control"
                  rows="4"
                  placeholder="Nhập nội dung..."
                  :class="{
                                'is-invalid':
                                  submitted && $v.modelTextSign.content.$error,
                                }"
              ></textarea>
              <div
                  v-if="submitted && !$v.modelTextSign.content.required"
                  class="invalid-feedback"
              >
               Nội dung không được để trống.
              </div>
            </div>
          </div>
        </div>
      </form>
      <template #modal-footer="{cancel}">
        <b-button class="btn bg-white me-2 text-primary px-5" @click="cancel()">Huỷ</b-button>
        <b-button class="btn px-3 text-white" @click="addTextSignature" style="background-color: #3082c9;">Thêm vào văn bản
        </b-button>
      </template>
    </b-modal>

    <!--    Modal hoành thành ký số -->
        <b-modal
            v-model="showModalHoanhThanhKySo"
            body-class="p-3 modal-hoanhthanhkyso"
            hide-header
            class="modal-hoanhthanhkyso"
            hide-footer
            centered
            no-close-on-backdrop
            hide-header-close
            size="md"

        >
          <div style="position: relative; ">

            <div class="modal-hoanhthanhkyso-box">
              <div class="title-hoanhthanhkyso mb-2">
                <p class="fs-5 fw-bold" style="color: #2a2a2a;">Xác nhận ký số tài liệu</p>
              </div>
              <div>
                <p class="fw-medium" style="color: #2a2a2a;">Yêu cầu ký số đã được gửi về thiết bị di động.</p>
                <p style="color: #2a2a2a;">Mở ứng dụng VNPT SmartCA và nhấn <b>Xác nhận</b> để ký tài liệu.</p>
              </div>
              <div class="row">
                <div class="col-lg-6 col-md-12 content-box-left mt-3">
                  <div
                      class="bg-xacnhankyso mb-3 w-100"
                      style="border-radius: 3px; padding: 5px;"
                  >
                    <div class="d-flex align-items-center justify-content-between">
                      <p class="fw-bold" style="color: #2a2a2a;">Ứng dụng</p>
                      <p class="me-3" style="color: #2a2a2a; font-style: italic">VNPT SmartCA</p>
                    </div>
                    <div class="d-flex align-items-center justify-content-between">
                      <p class="fw-bold" style="color: #2a2a2a;">Trạng thái</p>
                      <p class="me-3" style="color: #2a2a2a;">{{trangThaiKySo}}</p>
                    </div>
                  </div>
                  <div>
                    <p class="text-center" style="color: #2a2a2a;">Thời gian còn lại</p>
                    <div class="row">
                      <div class="col-12 px-3">
                        <Countdown :date.sync="end"></Countdown>
                      </div>
                      <div class="col-12 px-3" style="display: flex; justify-content: center; align-items: center">
                        <LetterCube  />
                      </div>

                    </div>
                  </div>
                  <div class="my-3">
                    <p style="color: #2a2a2a; font-style: italic">Hệ thống D-Office DThU tích hợp ký số VNPT SmartCA.</p>
                    <div style="display: flex; flex-direction: column">
                      <button class="btn text-primary px-4 fw-semibold me-2" v-if="downloadFile" @click="downloadPDF"   style="background-color: rgba(12,92,168, 0.1)"> Tải xuống</button>
                      <button class="btn text-info px-4 fw-semibold me-2" v-if="isFinish && trangThaiKySoCode == 'SUCCESS'" @click="handleCloseHoanThanhKySoModal" style="background-color: rgba(12,92,168, 0.1); margin-top: 10px"> Trở về</button>
                      <button class="btn text-info px-4 fw-semibold me-2" v-if="trangThaiKySoCode == 'ERROR'" @click="showModalHoanhThanhKySo = false" style="background-color: rgba(12,92,168, 0.1); margin-top: 10px"> Đóng</button>
                    </div>

                  </div>
                </div>
                <div
                    class="mt-3 col-lg-6 col-md-12 "

                >
                  <div
                      class="bg-xacnhankyso w-100 h-100 d-flex content-box-right align-items-center justify-content-center "
                      style="border-radius: 3px; padding: 5px;"
                  >
                    <img src="~@/assets/images/mobile-xacnhan.png" alt="mobile xac nhan" width="150">
                  </div>
                </div>
              </div>
            </div>

          </div>
        </b-modal>

<!-- Model Notify   -->
    <b-modal
        v-model="isShowModalAfterThietLap"
        title-class="text-black font-18"
        body-class="p-3"
        hide-footer
        hide-header
        centered
        no-close-on-backdrop
        size="md"
        style="padding: 0px"
    >
      <Transition name="fade" mode="out-in">
        <div class="row justify-content-center">
          <div class="col-md-12"
               style="display: flex; justify-content: center; align-items: center; padding: 40px 40px; flex-direction: column;">
            <!--                <i class='bx bx-check-circle h1 text-success' style="font-size: 100px; margin-bottom: 40px"></i>-->
            <div class="success-checkmark">
              <div class="check-icon">
                <span class="icon-line line-tip"></span>
                <span class="icon-line line-long"></span>
                <div class="icon-circle"></div>
                <div class="icon-fix"></div>
              </div>
            </div>
            <h1 v-if="resultData" class="fw-bold fs-3 text-dark text-uppercase">{{resultData.resultString}}</h1>
            <button type="button" v-on:click="handleHideModal" class="btn btn-modal text-uppercase"
                    style="width: 200px; margin-top: 20px; border-radius: 30px">Trờ về
            </button>
          </div>
        </div>
      </Transition>
    </b-modal>
  </div>
</template>
<style>
body {
  margin: 0;
  height: 100%;
  /*overflow: hidden;*/
}

.selected-pdf {
  box-shadow: 0 0 0 1px rgba(52, 117, 224, 0.5);
  border: 1px #14b0ef solid;
}

.slider-blue {
  --slider-connect-bg: #3475e0;
  --slider-tooltip-bg: #3475e0;
  --slider-handle-ring-color: #3475e0;
}

.nav-kyso-box {
  justify-content: flex-start;
}

.modal-hoanhthanhkyso {
  background-color: transparent !important;
}

@media only screen and (max-width: 425px) {
  .nav-kyso-box {
    flex-direction: column-reverse;
    justify-content: center;
  }

  .content-nav-left {
    justify-content: center;
  }

  .nav-kyso-right {
    justify-content: center !important;
  }

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

    &::before, &::after {
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
.kyso-box {
  & .card-kyso {

    & .card-body {
      padding: 0px;
      position: relative;

      & .nav-kyso {
        background-color: #0C5CA8;
        position: fixed;
        top: 70px;
        z-index: 1000;
        width: 100%;
        width: -webkit-fill-available;

        & .nav-kyso-box {
          & .nav-kyso-left {
            display: flex;
            align-items: center;
            padding: 5px;

            & .pdf-pages {
              display: flex;
              align-items: center;

              & .number-pages {
                & .btn-next {
                  background-color: #fff;
                }

                & .btn-prev {
                }
              }
            }

            & .pdf-zoom {
              display: flex;
              align-items: center;

              & .btn-number-zoom {
                background-color: #fff;
                color: #2a2a2a;
              }

            }

            & .nav-line {
              position: relative;

              &:before {
                content: "";
                display: block;
                width: 1px;
                height: 30px;
                color: #fff;
                background-color: #fff;
                margin: 0px 10px;
              }
            }

          }

          & .nav-kyso-right {
            justify-content: flex-end;

            & .dropdown {
              & .btn {
                background-color: #fff;
                border-radius: 5px;
              }

              & .dropdown-menu {
                & .btn-chuky,
                & .btn-kynhay,
                & .btn-kysophaply,
                & .btn-kynhay {
                  background-color: rgba(48, 130, 201, 0.1);
                  display: flex;
                  align-items: flex-end;
                  align-content: flex-end;
                  justify-content: center;
                  padding-top: 10px;
                  border-radius: 5px;
                }

                & .btn-kynhay {

                }

                & .btn-kysophaply {
                }
              }

              & .chuky-box {
                border: 1px dotted #ccc;
                height: 80px;
                min-height: 80px;
                width: 250px;
                min-width: 250px;
                border-radius: 5px;
                margin-top: 10px;
              }

              & .btn-selected {
                background-color: rgba(204, 204, 204, 0.5);
                border-top-right-radius: 0px;
                border-bottom-right-radius: 0px;

                &:hover {
                  background-color: #005990;
                  color: #fff;
                }
              }

              & .btn-delete {
                background-color: rgba(204, 204, 204, 0.2);
                border-top-left-radius: 0px;
                border-bottom-left-radius: 0px;

                &:hover {
                  background-color: #e72529;
                  color: #fff;
                }
              }
            }

            & .btn-ghichu,
            & .btn-kysophaply {
              background-color: #3082c9;
              color: #fff;
              margin: 10px 5px;
            }

            & .btn-download {
              background-color: #fff;
              border-color: #2a2a2a;
              margin: 10px 5px;
            }
          }
        }

      }
    }
  }
}

.btn-note-box {
  position: absolute;

}

.footer-kyso {
  position: fixed;
  bottom: 0;
  z-index: 1000;
  background-color: #f5f5f5;
  width: 100%;
  width: -webkit-fill-available;
  padding: 10px 0px;
}

.bg-xacnhankyso {
  background-color: #F7F6F9;
}

</style>


<style src="../../assets/tailwind.css"/>