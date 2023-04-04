<script>
import ObjectImage from "@/components/pdfEdtitorReview/objects/ObjectImage";
import TextEditor from "@/components/pdfEdtitorReview/Text"
import {mapState} from "vuex";

export default {
  components: {ObjectImage, TextEditor},
  props: {
    payload: {required: true},
    x: {required: true},
    y: {required: true},
    file: {required: false},
    width: {required: true},
    height: {required: true},
    pageScale: {required: true},
    opacity: {required: true},
    type: {required: true},
    path: {required: false, default: null},
    object: {
      required: true,
      type: Object,
    },
    imageBase64: {required: false}
  },
  data() {
    return {
      startX: 0,
      startY: 0,
      directions: [],
      dx: 0,
      dy: 0,
      dw: 0,
      dh: 0,
      pannableFunction: null,
      operation: null,
      signature: null,
      newX: 0,
      newY: 0
    }
  },
  computed: {
    moveOperation() {
      return this.operation === 'move'
    },
    getWidth(){
      let resultSize = this.width + this.dw;
      if(resultSize <= 0){
        resultSize = -1;
      }
      return resultSize;
    },
    ...mapState('pdfEditorStore', ['scale','oldScale'])
  },
  mounted() {
    this.setCanvas()
    console.log(`${this.object.x} - ${this.object.y}`)
  },
  watch:{
    scale(){
      this.setCanvas()
    }
  },
  methods: {
    changeSizeZoom(value, type){
      console.log(this.object.x, this.object.y)
      let sx = (Number(this.object.x) /this.oldScale) * Number(this.scale);
      let sy =  (Number(this.object.y) /this.oldScale) * Number(this.scale);


      let originalHeight =Math.round( value)/ this.oldScale;
      let data =  originalHeight * this.scale;
      if(type == "w"){
        this.newX = (Math.round(data) - Math.round(value));

      }
      if(type == "h"){
        this.newY = (Math.round(data) - Math.round(value));
      }
      this.$emit('update', {
        x: sx,
        y:sy,
      })
return data;

    },
    setCanvas() {
      if (this.type == 'image') {
        let cvn = this.$refs.canvasImage;
        // use canvas to prevent img tag's auto resize
        this.$refs.canvasImage.width = this.changeSizeZoom(this.width,"w");
        this.$refs.canvasImage.height = this.changeSizeZoom(this.height,"h");

        let w =  this.changeSizeZoom(this.width,"w");
        let h =  this.changeSizeZoom(this.height,"h");
        let ctx =  this.$refs.canvasImage.getContext("2d");
        let bg = new Image();
        bg.width = this.changeSizeZoom(this.width,"w");
        bg.height = this.changeSizeZoom(this.height,"h");
        bg.src = this.imageBase64;
        bg.onload = function() {
          ctx.drawImage(bg, 0 ,0, w,  h);
        };
        // let scale = this.scale
        // const limit = 500
        // if (this.width > limit) {
        //   scale = limit / this.width
        // }
        // if (this.height > limit) {
        //   scale = Math.min(scale, limit / this.height)
        // }
        this.$emit('update', {
          width: this.changeSizeZoom(this.width,"w"),
          height: this.changeSizeZoom(this.height,"h")
        })
      }else if (this.type == 'text') {
        // use canvas to prevent img tag's auto resize
        // this.$refs.canvasText.width = this.width
        // this.$refs.canvasText.height = this.height

        // let scale = 1
        // const limit = 500
        // if (this.width > limit) {
        //   scale = limit / this.width
        // }
        // if (this.height > limit) {
        //   scale = Math.min(scale, limit / this.height)
        // }
        // this.$emit('update', {
        //   width: this.width * scale,
        //   height: this.height * scale,
        // })
        this.$emit('update', {
          width: this.changeSizeZoom(this.width,"w"),
          height: this.changeSizeZoom(this.height,"h")
        })
      }
      else if (this.type == 'signature') {
        // this.value.setAttribute("viewBox", `0 0 200 200`);
      }
    },

    handlePanMove(event) {
      const _dx = (event.x - this.startX) / this.pageScale
      const _dy = (event.y - this.startY) / this.pageScale
      if (this.operation === 'move') {
        this.dx = _dx
        this.dy = _dy
      } else if (this.operation === 'scale') {
        // if (this.directions.includes('left')) {
        //   this.dx = _dx
        //   this.dw = -_dx
        // }
        // if (this.directions.includes('top')) {
        //   this.dy = _dy
        //   this.dh = -_dy
        // }
        if (this.directions.includes('right') && this.directions.includes('bottom')) {
          this.dw = _dx
           this.dh = _dy
        }
        // if (this.directions.includes('bottom')) {
        //
        // }
      }
    },

    handlePanEnd() {
      if (this.operation === 'move') {
        this.$emit('update', {
          x: this.x + this.dx,
          y: this.y + this.dy,
        })
        this.dx = 0
        this.dy = 0
      } else if (this.operation === 'scale') {
        this.$emit('update', {
          x: this.x + this.dx,
          y: this.y + this.dy,
          width: this.width + this.dw,
          height: this.height + this.dh,
        })

        this.dx = 0
        this.dy = 0
        this.dw = 0
        this.dh = 0
        this.directions = []
      }
      this.operation = ''
    },

    handlePanStart(event) {
      this.startX = event.x
      this.startY = event.y
      if (event.target === event.currentTarget) {
        return (this.operation = 'move')
      }
      this.operation = 'scale'
      this.directions = event.target.dataset.direction.split('-')
    },
    handleEndText(event){
      let value = {...this.object,...{
          lines: event.lines,
          lineHeight: event.lineHeight,
          size: event.size,
          fontFamily: event.fontFamily,
          height: event.height,
          width: event.width,
        }}

      this.$emit('update', value)
    },
  }

}

</script>
<template>
  <div

      class="tw-absolute tw-left-0 tw-top-0 tw-select-none"
      style="display: flex;justify-content: center; align-items: center;"
      :style="[getWidth == -1?{ height: `${Number(height) + Number(dh)}px`, transform: `translate(${Number(x) + Number(dx)}px, ${Number(y )+ Number(dy)}px)` } : { width: `${Number(width) + Number(dw)}px`, height: `${Number(height) + Number(dh)}px`, transform: `translate(${Number(x) + Number(dx)}px, ${Number(y) + Number(dy)}px)` }
      ,type=='text'?'display: flex;justify-content: center; align-items: center;':'']"
  >
    <object-image v-if="type== 'image'" :operation="operation" @panstart="handlePanStart" @panmove="handlePanMove" @panend="handlePanEnd"/>
    <TextEditor v-else-if="type == 'text'" :object="object" :text="object.text" :operation="operation" @panstart="handlePanStart" @panmove="handlePanMove" @panend="handlePanEnd"  @textEnd="handleEndText"/>
<!--   <div class="tw-absolute " style="display: flex; right: 0; top: -12px">-->
<!--&lt;!&ndash;     <div&ndash;&gt;-->
<!--&lt;!&ndash;         v-if="type== 'text'"&ndash;&gt;-->
<!--&lt;!&ndash;         @click="$emit('editText')"&ndash;&gt;-->
<!--&lt;!&ndash;         class="tw-left-0 tw-top-0 tw-right-0 tw-w-6 tw-h-6 tw-m-auto tw-rounded-full tw-bg-red-100 tw-cursor-pointer tw-transform tw&#45;&#45;translate-y-1/2 md:tw-scale-25 tw-text-center tw-border tw-border-black"&ndash;&gt;-->
<!--&lt;!&ndash;     >&ndash;&gt;-->
<!--&lt;!&ndash;       <i class="far fa-edit"></i>&ndash;&gt;-->
<!--&lt;!&ndash;     </div>&ndash;&gt;-->
<!--     <div-->
<!--         @click="$emit('delete')"-->
<!--         class="tw-left-0 tw-top-0 tw-right-0 tw-w-6 tw-h-6 tw-m-auto bg-white tw-cursor-pointer tw-transform tw&#45;&#45;translate-y-1/2 md:tw-scale-25 tw-text-center btn-remove-custom"-->
<!--     >-->
<!--&lt;!&ndash;       <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-x-circle"&ndash;&gt;-->
<!--&lt;!&ndash;            viewBox="0 0 16 16">&ndash;&gt;-->
<!--&lt;!&ndash;         <path d="M8 15A7 7 0 1 1 8 1a7 7 0 0 1 0 14zm0 1A8 8 0 1 0 8 0a8 8 0 0 0 0 16z"/>&ndash;&gt;-->
<!--&lt;!&ndash;         <path&ndash;&gt;-->
<!--&lt;!&ndash;             d="M4.646 4.646a.5.5 0 0 1 .708 0L8 7.293l2.646-2.647a.5.5 0 0 1 .708.708L8.707 8l2.647 2.646a.5.5 0 0 1-.708.708L8 8.707l-2.646 2.647a.5.5 0 0 1-.708-.708L7.293 8 4.646 5.354a.5.5 0 0 1 0-.708z"&ndash;&gt;-->
<!--&lt;!&ndash;         />&ndash;&gt;-->
<!--&lt;!&ndash;       </svg>&ndash;&gt;-->
<!--       <i class="far fa-window-close"></i>-->
<!--     </div>-->
<!--   </div>-->

    <canvas  v-if="type == 'image'" class="tw-w-full tw-h-full" ref="canvasImage"/>
  </div>
</template>

<style>
.btn-remove-custom{
  border: 1px dotted #8c8c91;
}
</style>