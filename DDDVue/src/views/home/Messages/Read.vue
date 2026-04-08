<template>
  <div class="read-message-container">
    <div class="read-message-content">
      <el-card class="message-card">
        <template #header>
          <div class="card-header">
            <div class="header-left">
              <span class="title-text">消息中心</span>
              <el-badge :value="unreadCount" :hidden="unreadCount === 0" class="unread-badge">
                <span class="unread-text">{{ unreadCount }} 条未读</span>
              </el-badge>
            </div>
            <div class="header-right">
              <el-button @click="closePage">返回</el-button>
              <el-button type="primary" @click="handleMarkAllRead" :disabled="unreadCount === 0">全部已读</el-button>
            </div>
          </div>
        </template>

        <!-- 筛选区域 -->
        <div class="filter-area">
          <el-select v-model="filterParams.messageType" placeholder="消息类型" clearable class="filter-select" @change="handleFilterChange">
            <el-option v-for="item in messageTypeOptions" :key="item.value" :label="item.label" :value="item.value" />
          </el-select>
          <el-select v-model="filterParams.priority" placeholder="优先级" clearable class="filter-select" @change="handleFilterChange">
            <el-option v-for="item in priorityOptions" :key="item.value" :label="item.label" :value="item.value" />
          </el-select>
          <el-select v-model="filterParams.onlyUnread" placeholder="阅读状态" clearable class="filter-select" @change="handleFilterChange">
            <el-option label="未读" :value="true" />
            <el-option label="已读" :value="false" />
          </el-select>
          <el-input v-model="filterParams.keyword" placeholder="搜索标题" clearable class="filter-input" @clear="handleFilterChange" @keyup.enter="handleFilterChange" />
          <el-button @click="handleReset">重置</el-button>
        </div>

        <!-- 消息列表 -->
        <el-table
          :data="messageList"
          style="width: 100%"
          :header-cell-style="{ background: '#f5f7fa', color: '#333' }"
          height="calc(100% - 120px)"
          @row-click="viewDetail"
          :row-class-name="getRowClassName"
        >
          <el-table-column label="作废状态" width="90">
            <template #default="{ row }">
              <el-tag v-if="row.isRevoked" type="info" effect="dark" size="small">已撤回</el-tag>
              <el-tag v-else type="success" effect="dark" size="small">正常</el-tag>
            </template>
          </el-table-column>
          <el-table-column prop="isRead" label="状态" width="80">
            <template #default="{ row }">
              <el-badge is-dot :type="row.isRead ? 'info' : 'danger'" class="read-badge">
                <span>{{ row.isRead ? '已读' : '未读' }}</span>
              </el-badge>
            </template>
          </el-table-column>
          <el-table-column prop="priority" label="优先级" width="90">
            <template #default="{ row }">
              <el-tag :type="getPriorityTag(row.priority)" effect="plain" size="small">
                {{ getPriorityLabel(row.priority) }}
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column prop="messageType" label="类型" width="90">
            <template #default="{ row }">
              <el-tag :type="row.messageType === 'System' ? 'info' : 'primary'" effect="plain" size="small">
                {{ getMessageTypeLabel(row.messageType) }}
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column prop="senderName" label="推送人" min-width="100">
            <template #default="{ row }">
              {{ row.senderName || '系统' }}
            </template>
          </el-table-column>
          <el-table-column prop="title" label="标题" min-width="200">
            <template #default="{ row }">
              <span :class="{ 'unread-title': !row.isRead, 'revoked-title': row.isRevoked }">{{ row.title }}</span>
            </template>
          </el-table-column>
          <el-table-column prop="createdAt" label="发送时间" min-width="170">
            <template #default="{ row }">
              {{ formatDateTime(row.createdAt) }}
            </template>
          </el-table-column>
          <el-table-column label="操作" width="80" fixed="right">
            <template #default="{ row }">
              <el-button size="small" type="primary" @click.stop="viewDetail(row)">查看</el-button>
            </template>
          </el-table-column>
        </el-table>

        <!-- 分页 -->
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

    <!-- 消息详情对话框 -->
    <el-dialog v-model="detailDialogVisible" title="消息详情" width="800px" :destroy-on-close="true">
      <el-descriptions :column="2" border>
        <el-descriptions-item label="推送人">{{ currentMessage?.senderName || '系统' }}</el-descriptions-item>
        <el-descriptions-item label="消息类型">
          <el-tag :type="currentMessage?.messageType === 'System' ? 'info' : 'primary'" effect="plain" size="small">
            {{ getMessageTypeLabel(currentMessage?.messageType) }}
          </el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="优先级">
          <el-tag :type="getPriorityTag(currentMessage?.priority)" effect="plain" size="small">
            {{ getPriorityLabel(currentMessage?.priority) }}
          </el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="发送时间">{{ formatDateTime(currentMessage?.createdAt) }}</el-descriptions-item>
        <el-descriptions-item label="阅读状态">
          <el-tag :type="currentMessage?.isRead ? 'success' : 'warning'" effect="dark" size="small">
            {{ currentMessage?.isRead ? '已读' : '未读' }}
          </el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="阅读时间">{{ currentMessage?.readTime ? formatDateTime(currentMessage?.readTime) : '-' }}</el-descriptions-item>
        <el-descriptions-item label="作废状态">
          <el-tag v-if="currentMessage?.isRevoked" type="info" effect="dark" size="small">已撤回</el-tag>
          <el-tag v-else type="success" effect="dark" size="small">正常</el-tag>
        </el-descriptions-item>
        <el-descriptions-item v-if="currentMessage?.revokedTime" label="撤回时间">
          {{ formatDateTime(currentMessage.revokedTime) }}
        </el-descriptions-item>
      </el-descriptions>

      <!-- 接收人阅读状态统计 -->
      <el-divider content-position="left">接收人阅读状态</el-divider>
      <div v-if="messageDetail" class="recipient-stats">
        <el-tag type="info" effect="plain">总接收人: {{ messageDetail.recipientCount }}</el-tag>
        <el-tag type="success" effect="plain">已读: {{ messageDetail.readCount }}</el-tag>
        <el-tag type="warning" effect="plain">未读: {{ messageDetail.unreadCount }}</el-tag>
      </div>

      <!-- 接收人列表表格 -->
      <el-table
        v-loading="recipientLoading"
        :data="recipientList"
        style="width: 100%; margin-top: 12px;"
        max-height="300"
        :header-cell-style="{ background: '#f5f7fa', color: '#333' }"
      >
        <el-table-column prop="recipientName" label="接收人" min-width="120" />
        <el-table-column label="阅读状态" width="100">
          <template #default="{ row }">
            <el-badge is-dot :type="row.isRead ? 'success' : 'danger'" class="read-badge">
              <span>{{ row.isRead ? '已读' : '未读' }}</span>
            </el-badge>
          </template>
        </el-table-column>
        <el-table-column label="阅读时间" min-width="170">
          <template #default="{ row }">
            {{ row.readTime ? formatDateTime(row.readTime) : '-' }}
          </template>
        </el-table-column>
        <el-table-column label="撤回状态" width="100">
          <template #default="{ row }">
            <el-tag v-if="row.isRevoked || currentMessage?.isRevoked" type="info" effect="dark" size="small">已撤回</el-tag>
            <el-tag v-else type="success" effect="dark" size="small">正常</el-tag>
          </template>
        </el-table-column>
      </el-table>

      <el-divider content-position="left">消息标题</el-divider>
      <div class="message-title">{{ currentMessage?.title }}</div>
      <el-divider content-position="left">消息内容</el-divider>
      <div class="message-content">{{ currentMessage?.content }}</div>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="detailDialogVisible = false">关闭</el-button>
          <el-button v-if="currentMessage && !currentMessage.isRead && !currentMessage.isRevoked" type="primary" @click="markCurrentAsRead">标记已读</el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import * as messageApi from '@/api/message'
import { getMessageDetail } from '@/api/message'
import type { UserMessageDto, MessageQueryRequest, MessageDetailDto, MessageRecipientDto } from '@/api/index'
import { showSuccessNotification, showErrorNotification } from '@/utils/notification'
import { useDictionaries, DICT_TYPES } from '@/utils/dictionary'

const { getMessages, getUserMessageById, markUserMessageAsRead, markAllAsRead: markAllAsReadApi } = messageApi

const router = useRouter()

const messageList = ref<UserMessageDto[]>([])
const unreadCount = ref(0)
const detailDialogVisible = ref(false)
const currentMessage = ref<UserMessageDto | null>(null)
const loading = ref(false)
const messageDetail = ref<MessageDetailDto | null>(null)
const recipientList = ref<MessageRecipientDto[]>([])
const recipientLoading = ref(false)

// 字典数据
const {
  dictDataMap,
  loadDicts,
  getLabelByValue
} = useDictionaries([
  DICT_TYPES.MESSAGE_TYPE,
  DICT_TYPES.MESSAGE_PRIORITY
])

// 字典选项
const messageTypeOptions = computed(() => dictDataMap.value[DICT_TYPES.MESSAGE_TYPE] || [])
const priorityOptions = computed(() => dictDataMap.value[DICT_TYPES.MESSAGE_PRIORITY] || [])

// 字典标签获取方法
const getMessageTypeLabel = (value?: string | number) => getLabelByValue(DICT_TYPES.MESSAGE_TYPE, value || '')
const getPriorityLabel = (value?: string | number) => getLabelByValue(DICT_TYPES.MESSAGE_PRIORITY, value || '')

interface Pagination {
  pageNum: number
  pageSize: number
  total: number
}

interface FilterParams {
  messageType: string
  priority: string
  onlyUnread: boolean | undefined
  keyword: string
}

const pagination = ref<Pagination>({
  pageNum: 1,
  pageSize: 10,
  total: 0
})

const filterParams = reactive<FilterParams>({
  messageType: '',
  priority: '',
  onlyUnread: undefined,
  keyword: ''
})

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

const getPriorityTag = (priority?: string) => {
  const tagMap: Record<string, string> = {
    Normal: 'info',
    Important: 'warning',
    Urgent: 'danger'
  }
  return tagMap[priority || 'Normal'] || 'info'
}

const getRowClassName = ({ row }: { row: UserMessageDto }) => {
  if (!row.isRead) {
    return 'unread-row'
  }
  return ''
}

const loadMessageData = async () => {
  loading.value = true
  try {
    const params: MessageQueryRequest = {
      pageNum: pagination.value.pageNum,
      pageSize: pagination.value.pageSize,
      messageType: filterParams.messageType || undefined,
      priority: filterParams.priority || undefined,
      onlyUnread: filterParams.onlyUnread,
      keyword: filterParams.keyword || undefined
    }

    const response = await getMessages(params)
    if (response.data) {
      messageList.value = response.data.list || []
      pagination.value.total = response.data.total || 0
    }
  } catch (error) {
    console.error('加载消息数据失败:', error)
    showErrorNotification({ title: '错误', message: '加载消息数据失败' })
  } finally {
    loading.value = false
  }
}

const loadUnreadCount = async () => {
  try {
    const response = await messageApi.getUnreadCount()
    if (response.data !== undefined) {
      unreadCount.value = response.data as number
    }
  } catch (error) {
    console.error('加载未读数量失败:', error)
  }
}

const handleFilterChange = () => {
  pagination.value.pageNum = 1
  loadMessageData()
}

const handleReset = () => {
  filterParams.messageType = ''
  filterParams.priority = ''
  filterParams.onlyUnread = undefined
  filterParams.keyword = ''
  pagination.value.pageNum = 1
  loadMessageData()
}

const viewDetail = async (row: UserMessageDto) => {
  try {
    // 使用 recipientId 获取消息详情
    const response = await getUserMessageById(row.recipientId)
    if (response.data) {
      currentMessage.value = response.data as UserMessageDto
      detailDialogVisible.value = true

      // 同时获取消息详情（包含接收人列表）
      recipientLoading.value = true
      try {
        const detailResponse = await getMessageDetail(row.messageId)
        if (detailResponse.data) {
          messageDetail.value = detailResponse.data as MessageDetailDto
          recipientList.value = messageDetail.value.recipients || []
        }
      } catch (err) {
        console.error('获取接收人列表失败:', err)
        // 如果没有权限获取接收人列表，不影响基本信息展示
        recipientList.value = []
      } finally {
        recipientLoading.value = false
      }

      // 如果是未读消息且未撤回，自动标记为已读
      if (!currentMessage.value.isRead && !currentMessage.value.isRevoked) {
        await markUserMessageAsRead(row.recipientId)
        await loadUnreadCount()
        await loadMessageData()
      }
    }
  } catch (error) {
    console.error('获取消息详情失败:', error)
    showErrorNotification({ title: '错误', message: '获取消息详情失败' })
  }
}

const markCurrentAsRead = async () => {
  if (currentMessage.value && !currentMessage.value.isRead && !currentMessage.value.isRevoked) {
    try {
      await markUserMessageAsRead(currentMessage.value.recipientId)
      showSuccessNotification({ title: '成功', message: '已标记为已读' })
      detailDialogVisible.value = false
      await loadUnreadCount()
      await loadMessageData()
    } catch (error) {
      console.error('操作失败:', error)
      showErrorNotification({ title: '错误', message: '操作失败' })
    }
  }
}

const handleMarkAllRead = async () => {
  try {
    await markAllAsReadApi()
    showSuccessNotification({ title: '成功', message: '已全部标记为已读' })
    await loadUnreadCount()
    await loadMessageData()
  } catch (error) {
    console.error('操作失败:', error)
    showErrorNotification({ title: '错误', message: '操作失败' })
  }
}

const handleSizeChange = (size: number) => {
  pagination.value.pageSize = size
  loadMessageData()
}

const handleCurrentChange = (page: number) => {
  pagination.value.pageNum = page
  loadMessageData()
}

const closePage = () => {
  router.push('/dashboard')
}

onMounted(async () => {
  await loadDicts()
  await loadMessageData()
  await loadUnreadCount()
})
</script>

<style scoped>
.read-message-container {
  padding: 0;
  height: 100%;
  background: #f5f7fa;
}

.read-message-content {
  margin: 0;
  height: 100%;
}

.message-card {
  height: 100%;
  border: none;
  border-radius: 0;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.header-left {
  display: flex;
  align-items: center;
  gap: 12px;
}

.title-text {
  font-size: 16px;
  font-weight: bold;
}

.unread-badge {
  display: flex;
  align-items: center;
}

.unread-text {
  font-size: 12px;
  color: #f56c6c;
  margin-left: 4px;
}

.header-right {
  display: flex;
  gap: 8px;
}

.filter-area {
  display: flex;
  gap: 12px;
  margin-bottom: 16px;
  flex-wrap: wrap;
}

.filter-select {
  width: 120px;
}

.filter-input {
  width: 150px;
}

.unread-row {
  background-color: #f0f9eb !important;
}

.unread-title {
  font-weight: bold;
  color: #409eff;
}

.revoked-title {
  text-decoration: line-through;
  color: #909399 !important;
  font-weight: normal !important;
}

.read-badge {
  margin-right: 8px;
}

.recipient-stats {
  display: flex;
  gap: 12px;
  margin-bottom: 12px;
}

.message-title {
  font-size: 16px;
  font-weight: bold;
  padding: 10px;
  background-color: #f5f7fa;
  border-radius: 4px;
}

.message-content {
  padding: 15px;
  background-color: #f5f7fa;
  border-radius: 4px;
  line-height: 1.8;
  white-space: pre-wrap;
}

.dialog-footer {
  text-align: right;
}
</style>