<template>
  <div class="dictionary-container">
    <div class="dictionary-content">
      <el-card class="dictionary-card">
        <template #header>
          <div class="card-header">
            <div class="filter-area">
              <el-input v-model="filterParams.code" placeholder="字典编码" clearable style="width: 120px" />
              <el-input v-model="filterParams.name" placeholder="字典名称" clearable style="width: 120px" />
              <el-input v-model="filterParams.type" placeholder="字典类型" clearable style="width: 120px" />
              <el-select v-model="filterParams.status" placeholder="状态" clearable style="width: 80px">
                <el-option v-for="item in statusOptions" :key="item.value" :label="item.label" :value="Number(item.value)" />
              </el-select>
              <el-button type="primary" @click="handleSearch">查询</el-button>
              <el-button @click="handleReset">重置</el-button>
              <el-divider direction="vertical" />
              <el-button v-if="hasBtn('dictionary:add')" type="primary" @click="addDictionary">添加字典</el-button>
            </div>
          </div>
        </template>
        <el-table :data="dictionaryList" style="width: 100%" :header-cell-style="{ background: '#f5f7fa', color: '#333' }"
          height="calc(100% - 120px)">
          <el-table-column prop="code" label="字典编码" min-width="120" />
          <el-table-column prop="name" label="字典名称" min-width="120" />
          <el-table-column prop="value" label="字典值" min-width="120" />
          <el-table-column prop="type" label="字典类型" width="100">
            <template #default="{ row }">
              <el-tag type="info" effect="plain">{{ row.type }}</el-tag>
            </template>
          </el-table-column>
          <el-table-column prop="sortOrder" label="排序" width="80" />
          <el-table-column prop="status" label="状态" width="80">
            <template #default="{ row }">
              <el-tag :type="row.status === 1 ? 'success' : 'danger'" effect="dark">
                {{ row.status === 1 ? '启用' : '禁用' }}
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column prop="description" label="描述" min-width="150">
            <template #default="{ row }">
              {{ row.description || '-' }}
            </template>
          </el-table-column>
          <el-table-column prop="createdAt" label="创建时间" min-width="170">
            <template #default="{ row }">
              {{ formatDateTime(row.createdAt) }}
            </template>
          </el-table-column>
          <el-table-column label="操作" width="200" fixed="right">
            <template #default="{ row }">
              <div class="table-actions">
                <el-button v-if="hasBtn('dictionary:edit')" size="small" @click="editDictionary(row)">编辑</el-button>
                <el-button v-if="hasBtn('dictionary:enable') && row.status !== 1" size="small" type="success" @click="toggleStatus(row)">启用</el-button>
                <el-button v-if="hasBtn('dictionary:disable') && row.status === 1" size="small" type="warning" @click="toggleStatus(row)">禁用</el-button>
                <el-button v-if="hasBtn('dictionary:delete')" size="small" type="danger" @click="deleteDictionaryItem(row)">删除</el-button>
              </div>
            </template>
          </el-table-column>
        </el-table>
        <el-pagination v-model:current-page="pagination.pageNum" v-model:page-size="pagination.pageSize"
          :total="pagination.total" :page-sizes="[10, 20, 50, 100]" layout="total, sizes, prev, pager, next, jumper"
          background style="margin-top: 16px; justify-content: flex-end" @size-change="handleSizeChange"
          @current-change="handleCurrentChange" />
      </el-card>
    </div>

    <!-- 字典编辑对话框 -->
    <el-dialog v-model="dialogVisible" :title="dialogTitle" width="500px" :destroy-on-close="true">
      <el-form :model="dictionaryForm" :rules="dictionaryRules" ref="dictionaryFormRef" label-width="100px">
        <el-form-item label="字典编码" prop="code">
          <el-input v-model="dictionaryForm.code" placeholder="请输入字典编码" :disabled="dialogTitle === '编辑字典'" />
        </el-form-item>
        <el-form-item label="字典名称" prop="name">
          <el-input v-model="dictionaryForm.name" placeholder="请输入字典名称" />
        </el-form-item>
        <el-form-item label="字典值" prop="value">
          <el-input v-model="dictionaryForm.value" placeholder="请输入字典值" />
        </el-form-item>
        <el-form-item label="字典类型" prop="type">
          <el-input v-model="dictionaryForm.type" placeholder="请输入字典类型" :disabled="dialogTitle === '编辑字典'" />
        </el-form-item>
        <el-form-item label="排序号">
          <el-input-number v-model="dictionaryForm.sortOrder" :min="0" :max="999" />
        </el-form-item>
        <el-form-item label="描述">
          <el-input v-model="dictionaryForm.description" type="textarea" :rows="2" placeholder="请输入描述" />
        </el-form-item>
        <el-form-item label="备注">
          <el-input v-model="dictionaryForm.remark" type="textarea" :rows="2" placeholder="请输入备注" />
        </el-form-item>
        <el-form-item label="状态" v-if="dialogTitle === '编辑字典'">
          <el-switch v-model="dictionaryForm.status" :active-value="1" :inactive-value="0" inline-prompt active-text="启用"
            inactive-text="禁用" active-color="#13ce66" inactive-color="#ff4949" />
        </el-form-item>
      </el-form>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="dialogVisible = false">取消</el-button>
          <el-button type="primary" @click="submitForm">确定</el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessageBox } from 'element-plus'
import * as dictionaryApi from '@/api/dictionary'
import type { DictionaryDto, CreateDictionaryRequest, UpdateDictionaryRequest } from '@/api/index'
import { showSuccessNotification, showErrorNotification } from '@/utils/notification'
import { getItem, setItem, removeItem, StorageKeys } from '@/utils/storage'
import { useButtons } from '@/utils/buttons'
import { useDictionary, DICT_TYPES, clearDictCache } from '@/utils/dictionary'

// 解构导入 API 函数
const {
  getDictionaries,
  createDictionary: createDictionaryApi,
  updateDictionary: updateDictionaryApi,
  deleteDictionary: deleteDictionaryApi,
  enableDictionary,
  disableDictionary
} = dictionaryApi

// 按钮管理
const { hasBtn } = useButtons('settings-dictionary')

// 状态字典
const { dictData: statusOptions, loadDict: loadStatusDict } = useDictionary(DICT_TYPES.STATUS)

// 字典表单类型
interface DictionaryForm {
  id?: string
  code: string
  name: string
  value: string
  type: string
  sortOrder: number
  description?: string
  remark?: string
  status: number
}

// 筛选参数
interface FilterParams {
  code: string
  name: string
  type: string
  status: number | undefined
}

// 分页数据
interface Pagination {
  pageNum: number
  pageSize: number
  total: number
}

// 响应式数据
const dictionaryList = ref<DictionaryDto[]>([])
const filterParams = reactive<FilterParams>({
  code: '',
  name: '',
  type: '',
  status: undefined
})
const dialogVisible = ref(false)
const dialogTitle = ref('添加字典')
const dictionaryFormRef = ref()
const dictionaryForm = ref<DictionaryForm>({
  code: '',
  name: '',
  value: '',
  type: '',
  sortOrder: 0,
  description: '',
  remark: '',
  status: 1
})

const pagination = ref<Pagination>({
  pageNum: 1,
  pageSize: 10,
  total: 0
})

// 字典表单验证规则
const dictionaryRules = {
  code: [
    { required: true, message: '请输入字典编码', trigger: 'blur' },
    { min: 1, max: 50, message: '字典编码长度在 1 到 50 个字符', trigger: 'blur' }
  ],
  name: [
    { required: true, message: '请输入字典名称', trigger: 'blur' },
    { min: 1, max: 50, message: '字典名称长度在 1 到 50 个字符', trigger: 'blur' }
  ],
  value: [
    { required: true, message: '请输入字典值', trigger: 'blur' }
  ],
  type: [
    { required: true, message: '请输入字典类型', trigger: 'blur' }
  ]
}

// 格式化日期时间
const formatDateTime = (dateStr: string) => {
  if (!dateStr) return '-'
  const date = new Date(dateStr)
  return date.toLocaleString('zh-CN', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit'
  })
}

// 加载字典数据
const loadDictionaryData = async () => {
  try {
    const params = {
      pageNum: pagination.value.pageNum,
      pageSize: pagination.value.pageSize,
      code: filterParams.code || undefined,
      name: filterParams.name || undefined,
      type: filterParams.type || undefined,
      status: filterParams.status
    }

    // 生成缓存键
    const cacheKey = `${StorageKeys.List}_dictionary_${params.pageNum}_${params.pageSize}`

    // 优先从缓存获取
    const cachedData = getItem<{ list: DictionaryDto[], total: number }>(cacheKey)
    if (cachedData) {
      dictionaryList.value = cachedData.list || []
      pagination.value.total = cachedData.total || 0
      return
    }

    // 缓存不存在，从 API 获取
    const response = await getDictionaries(params)
    if (response.data) {
      dictionaryList.value = response.data.list || []
      pagination.value.total = response.data.total || 0

      // 存入缓存
      setItem(cacheKey, {
        list: response.data.list,
        total: response.data.total
      })
    }
  } catch (error) {
    console.error('加载字典数据失败:', error)
    showErrorNotification({ title: '错误', message: '加载字典数据失败' })
  }
}

// 查询
const handleSearch = () => {
  pagination.value.pageNum = 1
  loadDictionaryData()
}

// 重置筛选
const handleReset = () => {
  filterParams.code = ''
  filterParams.name = ''
  filterParams.type = ''
  filterParams.status = undefined
  pagination.value.pageNum = 1
  loadDictionaryData()
}

// 添加字典
const addDictionary = () => {
  dialogTitle.value = '添加字典'
  resetForm()
  dialogVisible.value = true
}

// 编辑字典
const editDictionary = (row: DictionaryDto) => {
  dialogTitle.value = '编辑字典'
  dictionaryForm.value = {
    id: row.id,
    code: row.code,
    name: row.name,
    value: row.value,
    type: row.type,
    sortOrder: row.sortOrder,
    description: row.description || '',
    remark: row.remark || '',
    status: row.status
  }
  dialogVisible.value = true
}

// 重置表单
const resetForm = () => {
  dictionaryForm.value = {
    code: '',
    name: '',
    value: '',
    type: '',
    sortOrder: 0,
    description: '',
    remark: '',
    status: 1
  }
}

// 提交表单
const submitForm = async () => {
  try {
    await dictionaryFormRef.value.validate()

    // 清除缓存
    const cacheKey = `${StorageKeys.List}_dictionary_${pagination.value.pageNum}_${pagination.value.pageSize}`
    removeItem(cacheKey)

    if (dialogTitle.value === '添加字典') {
      const createData: CreateDictionaryRequest = {
        code: dictionaryForm.value.code,
        name: dictionaryForm.value.name,
        value: dictionaryForm.value.value,
        type: dictionaryForm.value.type,
        sortOrder: dictionaryForm.value.sortOrder,
        description: dictionaryForm.value.description,
        remark: dictionaryForm.value.remark
      }
      await createDictionaryApi(createData)
      // 清除该类型的字典缓存
      clearDictCache(dictionaryForm.value.type)
      showSuccessNotification({ title: '成功', message: '添加成功' })
    } else {
      const updateData: UpdateDictionaryRequest = {
        id: dictionaryForm.value.id!,
        name: dictionaryForm.value.name,
        value: dictionaryForm.value.value,
        type: dictionaryForm.value.type,
        sortOrder: dictionaryForm.value.sortOrder,
        description: dictionaryForm.value.description,
        remark: dictionaryForm.value.remark,
        status: dictionaryForm.value.status
      }
      await updateDictionaryApi(updateData)
      // 清除该类型的字典缓存
      clearDictCache(dictionaryForm.value.type)
      showSuccessNotification({ title: '成功', message: '编辑成功' })
    }

    dialogVisible.value = false
    await loadDictionaryData()
  } catch (error) {
    console.log('验证失败或保存失败:', error)
  }
}

// 切换状态
const toggleStatus = async (row: DictionaryDto) => {
  try {
    const action = row.status === 1 ? '禁用' : '启用'
    await ElMessageBox.confirm(`确认${action}字典 "${row.name}" 吗？`, '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })

    // 清除缓存
    const cacheKey = `${StorageKeys.List}_dictionary_${pagination.value.pageNum}_${pagination.value.pageSize}`
    removeItem(cacheKey)

    if (row.status === 1) {
      await disableDictionary(row.id)
    } else {
      await enableDictionary(row.id)
    }

    // 清除该类型的字典缓存
    clearDictCache(row.type)
    showSuccessNotification({ title: '成功', message: `${action}成功` })
    await loadDictionaryData()
  } catch (error) {
    console.log('取消操作或操作失败')
  }
}

// 删除字典
const deleteDictionaryItem = async (row: DictionaryDto) => {
  try {
    await ElMessageBox.confirm(`确认删除字典 "${row.name}" 吗？`, '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })

    // 清除缓存
    const cacheKey = `${StorageKeys.List}_dictionary_${pagination.value.pageNum}_${pagination.value.pageSize}`
    removeItem(cacheKey)

    await deleteDictionaryApi(row.id)
    // 清除该类型的字典缓存
    clearDictCache(row.type)
    showSuccessNotification({ title: '成功', message: '删除成功' })
    await loadDictionaryData()
  } catch (error) {
    console.log('取消删除或删除失败')
  }
}

// 处理分页大小变化
const handleSizeChange = (size: number) => {
  pagination.value.pageSize = size
  loadDictionaryData()
}

// 处理分页页码变化
const handleCurrentChange = (page: number) => {
  pagination.value.pageNum = page
  loadDictionaryData()
}

onMounted(() => {
  loadStatusDict()
  loadDictionaryData()
})
</script>

<style scoped>
.dictionary-container {
  padding: 0;
  height: 100%;
}

.dictionary-content {
  margin: 0;
  height: 100%;
}

.dictionary-card {
  height: 100%;
  border: none;
  border-radius: 0;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.filter-area {
  display: flex;
  gap: 12px;
  align-items: center;
  flex-wrap: wrap;
}

.table-actions {
  display: flex;
  gap: 8px;
  align-items: center;
}

.dialog-footer {
  text-align: right;
}
</style>