declare module '@wangeditor/editor-for-vue' {
  import { DefineComponent } from 'vue'
  
  export const Editor: DefineComponent<{
    defaultConfig?: any
    value?: string
    mode?: string
    style?: any
    onChange?: (editor: any) => void
    onCreated?: (editor: any) => void
  }>
  
  export const Toolbar: DefineComponent<{
    editor?: any
    defaultConfig?: any
    mode?: string
  }>
}

declare module '@wangeditor/editor' {
  export interface IDomEditor {
    getHtml(): string
    setHtml(html: string): void
    getText(): string
    clear(): void
    destroy(): void
  }
  
  export interface IEditorConfig {
    placeholder?: string
    MENU_CONF?: any
  }
  
  export interface IToolbarConfig {
    excludeKeys?: string[]
  }
}