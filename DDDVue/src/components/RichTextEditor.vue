<template>
  <div class="rich-text-editor">
    <Toolbar
      class="toolbar"
      :editor="editorRef"
      :defaultConfig="toolbarConfig"
      mode="simple"
    />
    <Editor
      class="editor"
      :style="{ height: height + 'px' }"
      :defaultConfig="editorConfig"
      :value="modelValue"
      @onChange="handleChange"
      @onCreated="handleCreated"
      mode="simple"
    />
  </div>
</template>

<script setup lang="ts">
import { shallowRef, onBeforeUnmount, watch } from 'vue'
import { Editor, Toolbar } from '@wangeditor/editor-for-vue'
// @ts-ignore - wangEditor 类型声明问题
import type { IDomEditor, IEditorConfig, IToolbarConfig } from '@wangeditor/editor'

interface Props {
  modelValue?: string
  height?: number
  placeholder?: string
}

interface Emits {
  (e: 'update:modelValue', value: string): void
  (e: 'change', value: string): void
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: '',
  height: 300,
  placeholder: '请输入内容...'
})

const emit = defineEmits<Emits>()

// 编辑器实例，必须用 shallowRef
const editorRef = shallowRef<IDomEditor>()

// 工具栏配置
const toolbarConfig: Partial<IToolbarConfig> = {
  excludeKeys: [
    'group-video',
    'fullScreen'
  ]
}

// 编辑器配置
const editorConfig: Partial<IEditorConfig> = {
  placeholder: props.placeholder,
  MENU_CONF: {
    uploadImage: {
      // 禁用图片上传
      disabled: true
    },
    uploadVideo: {
      // 禁用视频上传
      disabled: true
    }
  }
}

// 编辑器创建
const handleCreated = (editor: IDomEditor) => {
  editorRef.value = editor
}

// 内容变化
const handleChange = (editor: IDomEditor) => {
  const value = editor.getHtml()
  emit('update:modelValue', value)
  emit('change', value)
}

// 组件销毁时，也及时销毁编辑器
onBeforeUnmount(() => {
  const editor = editorRef.value
  if (editor == null) return
  editor.destroy()
})

// 监听外部值变化，同步到编辑器
watch(
  () => props.modelValue,
  (newValue) => {
    const editor = editorRef.value
    if (editor && newValue !== editor.getHtml()) {
      editor.setHtml(newValue || '')
    }
  }
)

// 获取纯文本内容
const getText = () => {
  const editor = editorRef.value
  if (editor) {
    return editor.getText()
  }
  return ''
}

// 清空内容
const clear = () => {
  const editor = editorRef.value
  if (editor) {
    editor.clear()
  }
}

// 暴露方法给父组件
defineExpose({
  getText,
  clear
})
</script>

<style src="@wangeditor/editor/dist/css/style.css"></style>

<style scoped>
.rich-text-editor {
  border: 1px solid #ccc;
  border-radius: 4px;
  overflow: hidden;
}

.toolbar {
  border-bottom: 1px solid #ccc;
}

.editor {
  overflow-y: auto;
}
</style>