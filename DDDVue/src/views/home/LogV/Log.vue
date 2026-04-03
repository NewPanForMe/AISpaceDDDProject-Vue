<template>
  <div class="log-container">
    <div class="log-content">
      <el-card class="log-card">
        <template #header>
          <div class="card-header">
            <div class="filter-area">
              <el-select v-model="filterParams.operationType" placeholder="操作类型" clearable class="filter-select" @change="handleFilterChange">
                <el-option v-for="item in operationTypeOptions" :key="item.value" :label="item.label" :value="item.value" />
              </el-select>
              <el-select v-model="filterParams.module" placeholder="操作模块" clearable class="filter-select" @change="handleFilterChange">
                <el-option v-for="item in moduleOptions" :key="item.value" :label="item.label" :value="item.value" />
              </el-select>
              <el-select v-model="filterParams.status" placeholder="状态" clearable class="filter-select-sm" @change="handleFilterChange">
                <el-option v-for="item in statusOptions" :key="item.value" :label="item.label" :value="item.value" />
              </el-select>
              <el-input v-model="filterParams.userName" placeholder="用户名" clearable class="filter-input" @clear="handleFilterChange" @keyup.enter="handleFilterChange" />
              <el-date-picker
                v-model="filterParams.timeRange"
                type="daterange"
                range-separator="至"
                start-placeholder="开始"
                end-placeholder="结束"
                value-format="YYYY-MM-DD"
                class="filter-date"
                @change="handleFilterChange"
              />
              <el-button @click="handleReset">重置</el-button>
              <el-divider direction="vertical" />
              <el-button v-if="hasBtn('log:export')" type="success" @click="handleExport">导出</el-button>
              <el-button v-if="hasBtn('log:clear')" type="warning" @click="openClearDialog">清空</el-button>
              <el-button v-if="hasBtn('log:delete')" type="danger" @click="handleBatchDelete" :disabled="selectedIds.length === 0">批量删除</el-button>
            </div>
          </div>
        </template>
        <el-table
          :data="logList"
          style="width: 100%"
          :header-cell-style="{ background: '#f5f7fa', color: '#333' }"
          height="calc(100% - 120px)"
          @selection-change="handleSelectionChange"
        >
          <el-table-column type="selection" width="50" />
          <el-table-column prop="userName" label="操作用户" min-width="100">
            <template #default="{ row }">
              {{ row.realName || row.userName }}
            </template>
          </el-table-column>
          <el-table-column prop="operationType" label="操作类型" width="100">
            <template #default="{ row }">
              <el-tag :type="getOperationTypeTag(row.operationType)" effect="plain">
                {{ getOperationTypeLabel(row.operationType) }}
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column prop="module" label="操作模块" width="100">
            <template #default="{ row }">
              {{ getModuleLabel(row.module) }}
            </template>
          </el-table-column>
          <el-table-column prop="description" label="操作描述" min-width="150">
            <template #default="{ row }">
              {{ row.description || '-' }}
            </template>
          </el-table-column>
          <el-table-column prop="requestMethod" label="请求方法" width="90">
            <template #default="{ row }">
              <el-tag :type="getMethodTag(row.requestMethod)" effect="plain" size="small">
                {{ row.requestMethod }}
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column prop="ipAddress" label="IP地址" min-width="130">
            <template #default="{ row }">
              {{ row.ipAddress || '-' }}
            </template>
          </el-table-column>
          <el-table-column prop="status" label="状态" width="80">
            <template #default="{ row }">
              <el-tag :type="row.status === 'Success' ? 'success' : 'danger'" effect="dark">
                {{ getStatusLabel(row.status) }}
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column prop="duration" label="耗时(ms)" width="100">
            <template #default="{ row }">
              <span :class="{ 'duration-slow': row.duration > 1000 }">{{ row.duration }}</span>
            </template>
          </el-table-column>
          <el-table-column prop="createdAt" label="操作时间" min-width="170">
            <template #default="{ row }">
              {{ formatDateTime(row.createdAt) }}
            </template>
          </el-table-column>
          <el-table-column label="操作" width="140" fixed="right">
            <template #default="{ row }">
              <div class="table-actions">
                <el-button size="small" type="primary" @click="viewDetail(row)">详情</el-button>
                <el-button v-if="hasBtn('log:delete')" size="small" type="danger" @click="handleDelete(row)">删除</el-button>
              </div>
            </template>
          </el-table-column>
        </el-table>
        <el-pagination
          v-model:current-page="pagination.pageNum"
          v-model:page-size="pagination.pageSize"
          :total="pagination.total"
          :page-sizes="[10, 20, 50, 100]"
          layout="total, sizes, prev, pager, next, jumper"
          background
          style="margin-top: 16px; justify-content: flex-end"
          @size-change="handleSizeChange"
          @current-change="handleCurrentChange"
        />
      </el-card>
    </div>

    <!-- 详情对话框 -->
    <el-dialog v-model="detailDialogVisible" title="日志详情" width="700px" :destroy-on-close="true">
      <el-descriptions :column="2" border>
        <el-descriptions-item label="操作用户">{{ currentLog?.realName || currentLog?.userName }}</el-descriptions-item>
        <el-descriptions-item label="操作类型">{{ getOperationTypeLabel(currentLog?.operationType) }}</el-descriptions-item>
        <el-descriptions-item label="操作模块">{{ getModuleLabel(currentLog?.module) }}</el-descriptions-item>
        <el-descriptions-item label="操作描述">{{ currentLog?.description || '-' }}</el-descriptions-item>
        <el-descriptions-item label="请求方法">{{ currentLog?.requestMethod }}</el-descriptions-item>
        <el-descriptions-item label="请求路径">{{ currentLog?.requestPath }}</el-descriptions-item>
        <el-descriptions-item label="IP地址">{{ currentLog?.ipAddress || '-' }}</el-descriptions-item>
        <el-descriptions-item label="执行状态">
          <el-tag :type="currentLog?.status === 'Success' ? 'success' : 'danger'" effect="dark">
            {{ getStatusLabel(currentLog?.status) }}
          </el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="执行耗时">{{ currentLog?.duration }} ms</el-descriptions-item>
        <el-descriptions-item label="操作时间">{{ formatDateTime(currentLog?.createdAt) }}</el-descriptions-item>
        <el-descriptions-item label="浏览器">{{ currentLog?.browser || '-' }}</el-descriptions-item>
        <el-descriptions-item label="操作系统">{{ currentLog?.osInfo || '-' }}</el-descriptions-item>
      </el-descriptions>
      <el-divider content-position="left">请求参数</el-divider>
      <el-input
        :value="formatJson(currentLog?.requestParams)"
        type="textarea"
        :rows="6"
        readonly
        class="json-textarea"
      />
      <el-divider content-position="left">响应结果</el-divider>
      <el-input
        :value="formatJson(currentLog?.responseResult)"
        type="textarea"
        :rows="6"
        readonly
        class="json-textarea"
      />
      <el-divider v-if="currentLog?.errorMessage" content-position="left">错误信息</el-divider>
      <el-input
        v-if="currentLog?.errorMessage"
        :value="currentLog?.errorMessage"
        type="textarea"
        :rows="3"
        readonly
      />
    </el-dialog>

    <!-- 清空日志对话框 -->
    <el-dialog v-model="clearDialogVisible" title="清空日志" width="400px" :destroy-on-close="true">
      <el-form :model="clearForm" label-width="100px">
        <el-form-item label="时间范围">
          <el-date-picker
            v-model="clearForm.timeRange"
            type="daterange"
            range-separator="至"
            start-placeholder="开始日期"
            end-placeholder="结束日期"
            value-format="YYYY-MM-DD"
            style="width: 100%"
          />
        </el-form-item>
        <el-form-item label="清空范围">
          <el-radio-group v-model="clearForm.clearType">
            <el-radio value="range">指定时间范围</el-radio>
            <el-radio value="all">全部日志</el-radio>
          </el-radio-group>
        </el-form-item>
      </el-form>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="clearDialogVisible = false">取消</el-button>
          <el-button type="danger" @click="handleClear">确认清空</el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessageBox } from 'element-plus'
import * as logApi from '@/api/log'
import type { OperationLogDto, OperationLogQueryRequest, ClearLogsRequest } from '@/api/index'
import { showSuccessNotification, showErrorNotification } from '@/utils/notification'
import { useButtons } from '@/utils/buttons'
import { useDictionaries, DICT_TYPES } from '@/utils/dictionary'

// 解构导入 API 函数
const {
  getOperationLogs,
  deleteOperationLog,
  batchDeleteOperationLogs,
  clearOperationLogs,
  exportOperationLogs
} = logApi

// 按钮管理
const { hasBtn } = useButtons('logs')

// 字典数据 - 批量获取
const {
  dictDataMap,
  loadDicts,
  getLabelByValue
} = useDictionaries([
  DICT_TYPES.LOG_OPERATION_TYPE,
  DICT_TYPES.LOG_MODULE,
  DICT_TYPES.LOG_STATUS
])

// 字典选项 - 使用 computed 响应式获取
const operationTypeOptions = computed(() => dictDataMap.value[DICT_TYPES.LOG_OPERATION_TYPE] || [])
const moduleOptions = computed(() => dictDataMap.value[DICT_TYPES.LOG_MODULE] || [])
const statusOptions = computed(() => dictDataMap.value[DICT_TYPES.LOG_STATUS] || [])

// 字典标签获取方法
const getOperationTypeLabel = (value?: string | number) => getLabelByValue(DICT_TYPES.LOG_OPERATION_TYPE, value || '')
const getModuleLabel = (value?: string | number) => getLabelByValue(DICT_TYPES.LOG_MODULE, value || '')
const getStatusLabel = (value?: string | number) => getLabelByValue(DICT_TYPES.LOG_STATUS, value || '')

// 分页数据
interface Pagination {
  pageNum: number
  pageSize: number
  total: number
}

// 筛选参数
interface FilterParams {
  operationType: string
  module: string
  status: string
  userName: string
  timeRange: string[]
}

// 响应式数据
const logList = ref<OperationLogDto[]>([])
const selectedIds = ref<string[]>([])
const pagination = ref<Pagination>({
  pageNum: 1,
  pageSize: 10,
  total: 0
})

const filterParams = reactive<FilterParams>({
  operationType: '',
  module: '',
  status: '',
  userName: '',
  timeRange: []
})

// 详情对话框
const detailDialogVisible = ref(false)
const currentLog = ref<OperationLogDto | null>(null)

// 清空对话框
const clearDialogVisible = ref(false)
const clearForm = reactive({
  timeRange: [] as string[],
  clearType: 'range' as 'range' | 'all'
})

// 格式化日期时间
const formatDateTime = (dateStr?: string) => {
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

// 格式化 JSON 字符串
const formatJson = (jsonStr?: string) => {
  if (!jsonStr) return '无数据'
  try {
    const parsed = JSON.parse(jsonStr)
    return JSON.stringify(parsed, null, 2)
  } catch {
    return jsonStr
  }
}

// 操作类型标签颜色
const getOperationTypeTag = (type: string) => {
  const tagMap: Record<string, string> = {
    Create: 'success',
    Update: 'warning',
    Delete: 'danger',
    Export: 'info',
    Import: 'info',
    Enable: 'success',
    Disable: 'warning',
    Login: 'primary',
    Logout: 'info',
    Assign: 'warning',
    Other: ''
  }
  return tagMap[type] || ''
}

// 请求方法标签颜色
const getMethodTag = (method: string) => {
  const tagMap: Record<string, string> = {
    GET: 'success',
    POST: 'primary',
    PUT: 'warning',
    DELETE: 'danger'
  }
  return tagMap[method] || 'info'
}

// 加载日志数据
const loadLogData = async () => {
  try {
    const params: OperationLogQueryRequest = {
      pageNum: pagination.value.pageNum,
      pageSize: pagination.value.pageSize,
      operationType: filterParams.operationType || undefined,
      module: filterParams.module || undefined,
      status: filterParams.status || undefined,
      userName: filterParams.userName || undefined,
      startTime: filterParams.timeRange?.[0] || undefined,
      endTime: filterParams.timeRange?.[1] || undefined
    }

    const response = await getOperationLogs(params)
    if (response.data) {
      logList.value = response.data.list || []
      pagination.value.total = response.data.total || 0
    }
  } catch (error) {
    console.error('加载日志数据失败:', error)
    showErrorNotification({ title: '错误', message: '加载日志数据失败' })
  }
}

// 筛选条件变化时自动刷新
const handleFilterChange = () => {
  pagination.value.pageNum = 1
  loadLogData()
}

// 重置筛选
const handleReset = () => {
  filterParams.operationType = ''
  filterParams.module = ''
  filterParams.status = ''
  filterParams.userName = ''
  filterParams.timeRange = []
  pagination.value.pageNum = 1
  loadLogData()
}

// 选择变化
const handleSelectionChange = (selection: OperationLogDto[]) => {
  selectedIds.value = selection.map(item => item.id)
}

// 查看详情
const viewDetail = (row: OperationLogDto) => {
  currentLog.value = row
  detailDialogVisible.value = true
}

// 删除单条日志
const handleDelete = async (row: OperationLogDto) => {
  try {
    await ElMessageBox.confirm('确认删除该日志记录吗？', '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })

    await deleteOperationLog(row.id)
    showSuccessNotification({ title: '成功', message: '删除成功' })
    await loadLogData()
  } catch (error) {
    console.log('取消删除或删除失败')
  }
}

// 批量删除
const handleBatchDelete = async () => {
  try {
    await ElMessageBox.confirm(`确认删除选中的 ${selectedIds.value.length} 条日志记录吗？`, '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })

    await batchDeleteOperationLogs(selectedIds.value)
    showSuccessNotification({ title: '成功', message: '批量删除成功' })
    selectedIds.value = []
    await loadLogData()
  } catch (error) {
    console.log('取消删除或删除失败')
  }
}

// 打开清空对话框
const openClearDialog = () => {
  clearForm.timeRange = []
  clearForm.clearType = 'range'
  clearDialogVisible.value = true
}

// 清空日志
const handleClear = async () => {
  try {
    const actionText = clearForm.clearType === 'all' ? '全部' : '指定时间范围内的'
    await ElMessageBox.confirm(`确认清空${actionText}日志吗？此操作不可恢复！`, '警告', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })

    const data: ClearLogsRequest = {}
    if (clearForm.clearType === 'range' && clearForm.timeRange?.length === 2) {
      data.startTime = clearForm.timeRange[0]
      data.endTime = clearForm.timeRange[1]
    }

    await clearOperationLogs(data)
    showSuccessNotification({ title: '成功', message: '清空成功' })
    clearDialogVisible.value = false
    await loadLogData()
  } catch (error) {
    console.log('取消清空或清空失败')
  }
}

// 导出日志
const handleExport = async () => {
  try {
    const params: OperationLogQueryRequest = {
      pageNum: 1,
      pageSize: 10000,
      operationType: filterParams.operationType || undefined,
      module: filterParams.module || undefined,
      status: filterParams.status || undefined,
      userName: filterParams.userName || undefined,
      startTime: filterParams.timeRange?.[0] || undefined,
      endTime: filterParams.timeRange?.[1] || undefined
    }

    const blob = await exportOperationLogs(params)
    if (blob) {
      // 创建下载链接
      const url = window.URL.createObjectURL(blob)
      const link = document.createElement('a')
      link.href = url
      link.download = `操作日志_${new Date().toISOString().slice(0, 10)}.xlsx`
      document.body.appendChild(link)
      link.click()
      document.body.removeChild(link)
      window.URL.revokeObjectURL(url)
      showSuccessNotification({ title: '成功', message: '导出成功' })
    }
  } catch (error) {
    console.error('导出失败:', error)
    showErrorNotification({ title: '错误', message: '导出失败' })
  }
}

// 处理分页大小变化
const handleSizeChange = (size: number) => {
  pagination.value.pageSize = size
  loadLogData()
}

// 处理分页页码变化
const handleCurrentChange = (page: number) => {
  pagination.value.pageNum = page
  loadLogData()
}

onMounted(async () => {
  // 批量加载字典数据
  await loadDicts()
  // 加载日志数据
  await loadLogData()
})
</script>

<style scoped>
.log-container {
  padding: 0;
  height: 100%;
}

.log-content {
  margin: 0;
  height: 100%;
}

.log-card {
  height: 100%;
  border: none;
  border-radius: 0;
}

.card-header {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.filter-area {
  display: flex;
  gap: 12px;
  align-items: center;
  flex-wrap: wrap;
}

/* 筛选组件样式 */
.filter-select {
  width: 120px;
}

.filter-select-sm {
  width: 90px;
}

.filter-input {
  width: 120px;
}

.filter-date {
  width: 240px;
}

.table-actions {
  display: flex;
  gap: 8px;
  align-items: center;
}

.duration-slow {
  color: #f56c6c;
  font-weight: bold;
}

.dialog-footer {
  text-align: right;
}

/* JSON 文本框样式 */
.json-textarea :deep(textarea) {
  font-family: 'Consolas', 'Monaco', 'Courier New', monospace;
  font-size: 13px;
  line-height: 1.5;
}
</style>