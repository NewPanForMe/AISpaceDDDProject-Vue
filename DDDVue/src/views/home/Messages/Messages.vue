<template>
  <div class="messages-container">
    <div class="messages-content">
      <el-card class="messages-card">
        <template #header>
          <div class="card-header">
            <div class="header-left">
              <span class="title-text">站内信</span>
            </div>
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
              <el-divider direction="vertical" />
              <el-button v-if="hasBtn('message:push')" type="success" @click="openPushDialog">新建推送</el-button>
              <el-button type="primary" @click="handleMarkAllRead" :disabled="unreadCount === 0">全部已读</el-button>
              <el-button v-if="hasBtn('message:delete')" type="danger" @click="handleBatchDelete" :disabled="selectedIds.length === 0 || !hasDeletableSelected">批量删除</el-button>
              <el-button v-if="hasBtn('message:revoke')" type="warning" @click="handleBatchRevoke" :disabled="selectedIds.length === 0">批量撤回</el-button>
            </div>
          </div>
        </template>
        <el-table
          :data="messageList"
          style="width: 100%"
          :header-cell-style="{ background: '#f5f7fa', color: '#333' }"
          height="calc(100% - 120px)"
          @selection-change="handleSelectionChange"
          :row-class-name="getRowClassName"
        >
          <el-table-column type="selection" width="50" />
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
          <el-table-column prop="senderName" label="发送者" min-width="100">
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
          <el-table-column label="操作" width="280" fixed="right">
            <template #default="{ row }">
              <div class="table-actions">
                <el-button size="small" type="primary" @click="viewDetail(row)">查看</el-button>
                <el-button v-if="!row.isRead && !row.isRevoked" size="small" type="success" @click="markSingleAsRead(row)">已读</el-button>
                <el-button v-if="hasBtn('message:push') && !row.isPushed" size="small" type="success" @click="openPushExistingDialog(row)">推送</el-button>
                <el-button v-if="hasBtn('message:edit') && !row.isPushed" size="small" type="warning" @click="openEditDialog(row)">修改</el-button>
                <el-button v-if="hasBtn('message:revoke') && !row.isRevoked" size="small" type="warning" @click="handleRevoke(row)">撤回</el-button>
                <el-button v-if="hasBtn('message:delete') && !row.isRead && !row.hasBeenReadByOthers" size="small" type="danger" @click="handleDelete(row)">删除</el-button>
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
    <el-dialog v-model="detailDialogVisible" title="消息详情" width="800px" :destroy-on-close="true">
      <el-descriptions :column="2" border>
        <el-descriptions-item label="发送者">{{ currentMessage?.senderName || '系统' }}</el-descriptions-item>
        <el-descriptions-item label="消息状态">
          <el-tag v-if="currentMessage?.isRevoked" type="info" effect="dark" size="small">已撤回</el-tag>
          <el-tag v-else type="success" effect="dark" size="small">正常</el-tag>
        </el-descriptions-item>
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
        <el-descriptions-item v-if="currentMessage?.isRevoked" label="撤回时间">{{ formatDateTime(currentMessage?.revokedTime) }}</el-descriptions-item>
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
      <div :class="{ 'message-title': true, 'revoked-message': currentMessage?.isRevoked }">{{ currentMessage?.title }}</div>
      <el-divider content-position="left">消息内容</el-divider>
      <div :class="{ 'message-content': true, 'message-content-html': true, 'revoked-message': currentMessage?.isRevoked }" v-html="currentMessage?.content"></div>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="detailDialogVisible = false">关闭</el-button>
          <el-button v-if="currentMessage && !currentMessage.isRead && !currentMessage.isRevoked" type="primary" @click="markCurrentAsRead">标记已读</el-button>
        </span>
      </template>
    </el-dialog>

    <!-- 修改消息对话框 -->
    <el-dialog v-model="editDialogVisible" title="修改消息" width="700px" :destroy-on-close="true">
      <el-form :model="editForm" :rules="messageRules" ref="editFormRef" label-width="80px">
        <el-form-item label="消息标题" prop="title">
          <el-input v-model="editForm.title" placeholder="请输入消息标题" maxlength="100" show-word-limit />
        </el-form-item>
        <el-form-item label="消息内容" prop="content">
          <RichTextEditor v-model="editForm.content" :height="250" placeholder="请输入消息内容" />
        </el-form-item>
        <el-form-item label="优先级" prop="priority">
          <el-select v-model="editForm.priority" placeholder="选择优先级" class="full-width">
            <el-option v-for="item in priorityOptions" :key="item.value" :label="item.label" :value="item.value" />
          </el-select>
        </el-form-item>
      </el-form>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="editDialogVisible = false">取消</el-button>
          <el-button type="primary" @click="handleEdit">保存</el-button>
        </span>
      </template>
    </el-dialog>

    <!-- 推送消息对话框 -->
    <el-dialog v-model="pushDialogVisible" title="推送系统消息" width="700px" :destroy-on-close="true">
      <el-form :model="pushForm" :rules="pushRules" ref="pushFormRef" label-width="80px">
        <el-form-item label="推送范围" prop="pushType">
          <el-radio-group v-model="pushForm.pushType">
            <el-radio value="all">所有用户</el-radio>
            <el-radio value="role">指定角色</el-radio>
          </el-radio-group>
        </el-form-item>
        <el-form-item v-if="pushForm.pushType === 'role'" label="选择角色" prop="roleIds">
          <el-select v-model="pushForm.roleIds" multiple placeholder="选择角色" filterable class="full-width">
            <el-option v-for="role in roleList" :key="role.id" :label="role.name" :value="role.id" />
          </el-select>
        </el-form-item>
        <el-form-item label="消息标题" prop="title">
          <el-input v-model="pushForm.title" placeholder="请输入消息标题" maxlength="100" show-word-limit />
        </el-form-item>
        <el-form-item label="消息内容" prop="content">
          <RichTextEditor v-model="pushForm.content" :height="250" placeholder="请输入消息内容" />
        </el-form-item>
        <el-form-item label="优先级" prop="priority">
          <el-select v-model="pushForm.priority" placeholder="选择优先级" class="full-width">
            <el-option v-for="item in priorityOptions" :key="item.value" :label="item.label" :value="item.value" />
          </el-select>
        </el-form-item>
      </el-form>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="pushDialogVisible = false">取消</el-button>
          <el-button type="primary" @click="handlePush">推送</el-button>
        </span>
      </template>
    </el-dialog>

    <!-- 推送已有消息对话框 -->
    <el-dialog v-model="pushExistingDialogVisible" title="推送消息" width="500px" :destroy-on-close="true">
      <el-descriptions :column="1" border style="margin-bottom: 16px">
        <el-descriptions-item label="消息标题">{{ currentPushMessage?.title }}</el-descriptions-item>
        <el-descriptions-item label="消息内容">
          <div class="content-preview content-preview-html" v-html="currentPushMessage?.content"></div>
        </el-descriptions-item>
      </el-descriptions>
      <el-form :model="pushExistingForm" :rules="pushExistingRules" ref="pushExistingFormRef" label-width="80px">
        <el-form-item label="推送范围" prop="pushType">
          <el-radio-group v-model="pushExistingForm.pushType">
            <el-radio value="all">所有用户</el-radio>
            <el-radio value="role">指定角色</el-radio>
            <el-radio value="user">指定用户</el-radio>
          </el-radio-group>
        </el-form-item>
        <el-form-item v-if="pushExistingForm.pushType === 'role'" label="选择角色" prop="roleIds">
          <el-select v-model="pushExistingForm.roleIds" multiple placeholder="选择角色" filterable class="full-width">
            <el-option v-for="role in roleList" :key="role.id" :label="role.name" :value="role.id" />
          </el-select>
        </el-form-item>
        <el-form-item v-if="pushExistingForm.pushType === 'user'" label="选择用户" prop="userIds">
          <el-select v-model="pushExistingForm.userIds" multiple placeholder="选择用户" filterable class="full-width">
            <el-option v-for="user in userList" :key="user.id" :label="user.realName || user.userName" :value="user.id" />
          </el-select>
        </el-form-item>
      </el-form>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="pushExistingDialogVisible = false">取消</el-button>
          <el-button type="primary" @click="handlePushExisting">推送</el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessageBox, type FormInstance, type FormRules } from 'element-plus'
import * as messageApi from '@/api/message'
import { revokeMessage, batchRevokeMessages, getMessageDetail } from '@/api/message'
import * as userApi from '@/api/user'
import * as roleApi from '@/api/role'
import type { UserMessageDto, MessageQueryRequest, UserDto, RoleDto, UpdateMessageRequest, PushMessageRequest, PushMessageToRoleRequest, PushExistingMessageRequest, MessageDetailDto, MessageRecipientDto } from '@/api/index'
import { showSuccessNotification, showErrorNotification } from '@/utils/notification'
import { useButtons } from '@/utils/buttons'
import { useDictionaries, DICT_TYPES } from '@/utils/dictionary'
import RichTextEditor from '@/components/RichTextEditor.vue'

const {
  getMessages,
  getUserMessageById,
  markUserMessageAsRead,
  markAllAsRead,
  deleteUserMessage,
  batchDeleteUserMessages,
  getUnreadCount,
  updateMessage,
  pushMessageToAll,
  pushMessageToRole,
  pushExistingMessage
} = messageApi

const { hasBtn } = useButtons('messages')

// 字典数据 - 批量获取
const {
  dictDataMap,
  loadDicts,
  getLabelByValue
} = useDictionaries([
  DICT_TYPES.MESSAGE_TYPE,
  DICT_TYPES.MESSAGE_PRIORITY
])

// 字典选项 - 使用 computed 响应式获取
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

const messageList = ref<UserMessageDto[]>([])
const selectedIds = ref<string[]>([])
const selectedMessages = ref<UserMessageDto[]>([])
const pagination = ref<Pagination>({
  pageNum: 1,
  pageSize: 10,
  total: 0
})
const unreadCount = ref(0)

// 选中的消息中是否有可删除的消息（未读且没有其他用户已读）
const hasDeletableSelected = computed(() => {
  return selectedMessages.value.some(msg => !msg.isRead && !msg.hasBeenReadByOthers)
})

// 用户和角色列表
const userList = ref<UserDto[]>([])
const roleList = ref<RoleDto[]>([])

const filterParams = reactive<FilterParams>({
  messageType: '',
  priority: '',
  onlyUnread: undefined,
  keyword: ''
})

const detailDialogVisible = ref(false)
const currentMessage = ref<UserMessageDto | null>(null)
const messageDetail = ref<MessageDetailDto | null>(null)
const recipientList = ref<MessageRecipientDto[]>([])
const recipientLoading = ref(false)

// 修改消息表单
const editDialogVisible = ref(false)
const editFormRef = ref<FormInstance>()
const editForm = reactive<UpdateMessageRequest>({
  title: '',
  content: '',
  priority: 'Normal'
})
const editMessageId = ref('')

// 推送消息表单
const pushDialogVisible = ref(false)
const pushFormRef = ref<FormInstance>()
const pushForm = reactive({
  pushType: 'all' as 'all' | 'role',
  roleIds: [] as string[],
  title: '',
  content: '',
  priority: 'Normal'
})

const getPlainTextFromHtml = (html?: string) => {
  if (!html) return ''

  return html
    .replace(/<[^>]*>/g, '')
    .replace(/&nbsp;/gi, ' ')
    .trim()
}

const validateMessageContent = (_rule: unknown, value: string, callback: (error?: Error) => void) => {
  if (!value || value.length > 4000) {
    callback(new Error('消息内容长度需在 1-4000 字符之间'))
    return
  }

  const plainText = getPlainTextFromHtml(value)
  if (!plainText) {
    callback(new Error('请输入消息内容'))
    return
  }

  callback()
}

// 表单验证规则
const messageRules: FormRules = {
  title: [{ required: true, message: '请输入消息标题', trigger: 'blur' }],
  content: [{ validator: validateMessageContent, trigger: 'change' }],
  priority: [{ required: true, message: '请选择优先级', trigger: 'change' }]
}

const pushRules: FormRules = {
  pushType: [{ required: true, message: '请选择推送范围', trigger: 'change' }],
  roleIds: [
    {
      validator: (_rule, value, callback) => {
        if (pushForm.pushType === 'role' && (!value || value.length === 0)) {
          callback(new Error('请选择角色'))
        } else {
          callback()
        }
      },
      trigger: 'change'
    }
  ],
  title: [{ required: true, message: '请输入消息标题', trigger: 'blur' }],
  content: [{ validator: validateMessageContent, trigger: 'change' }],
  priority: [{ required: true, message: '请选择优先级', trigger: 'change' }]
}

// 推送已有消息表单
const pushExistingDialogVisible = ref(false)
const pushExistingFormRef = ref<FormInstance>()
const currentPushMessage = ref<UserMessageDto | null>(null)
const pushExistingForm = reactive<PushExistingMessageRequest>({
  pushType: 'all',
  roleIds: [],
  userIds: []
})

const pushExistingRules: FormRules = {
  pushType: [{ required: true, message: '请选择推送范围', trigger: 'change' }],
  roleIds: [
    {
      validator: (_rule, value, callback) => {
        if (pushExistingForm.pushType === 'role' && (!value || value.length === 0)) {
          callback(new Error('请选择角色'))
        } else {
          callback()
        }
      },
      trigger: 'change'
    }
  ],
  userIds: [
    {
      validator: (_rule, value, callback) => {
        if (pushExistingForm.pushType === 'user' && (!value || value.length === 0)) {
          callback(new Error('请选择用户'))
        } else {
          callback()
        }
      },
      trigger: 'change'
    }
  ]
}

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

// 加载用户列表
const loadUserList = async () => {
  try {
    const response = await userApi.getUsers({ pageNum: 1, pageSize: 1000 })
    if (response.data) {
      userList.value = response.data.list || []
    }
  } catch (error) {
    console.error('加载用户列表失败:', error)
  }
}

// 加载角色列表
const loadRoleList = async () => {
  try {
    const response = await roleApi.getEnabledRoles()
    if (response.data) {
      roleList.value = response.data as RoleDto[]
    }
  } catch (error) {
    console.error('加载角色列表失败:', error)
  }
}

const loadMessageData = async () => {
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
  }
}

const loadUnreadCount = async () => {
  try {
    const response = await getUnreadCount()
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

const handleSelectionChange = (selection: UserMessageDto[]) => {
  selectedIds.value = selection.map(item => item.recipientId)
  selectedMessages.value = selection
}

// 打开修改对话框
const openEditDialog = (row: UserMessageDto) => {
  editMessageId.value = row.messageId
  editForm.title = row.title
  editForm.content = row.content
  editForm.priority = row.priority
  editDialogVisible.value = true
}

// 打开推送对话框
const openPushDialog = () => {
  pushForm.pushType = 'all'
  pushForm.roleIds = []
  pushForm.title = ''
  pushForm.content = ''
  pushForm.priority = 'Normal'
  pushDialogVisible.value = true
}

// 打开推送已有消息对话框
const openPushExistingDialog = (row: UserMessageDto) => {
  currentPushMessage.value = row
  pushExistingForm.pushType = 'all'
  pushExistingForm.roleIds = []
  pushExistingForm.userIds = []
  pushExistingDialogVisible.value = true
}

// 修改消息
const handleEdit = async () => {
  if (!editFormRef.value) return
  await editFormRef.value.validate(async (valid) => {
    if (valid) {
      try {
        await updateMessage(editMessageId.value, editForm)
        showSuccessNotification({ title: '成功', message: '消息修改成功' })
        editDialogVisible.value = false
        await loadMessageData()
      } catch (error) {
        console.error('修改消息失败:', error)
        showErrorNotification({ title: '错误', message: '修改消息失败' })
      }
    }
  })
}

// 推送消息
const handlePush = async () => {
  if (!pushFormRef.value) return
  await pushFormRef.value.validate(async (valid) => {
    if (valid) {
      try {
        if (pushForm.pushType === 'all') {
          const data: PushMessageRequest = {
            title: pushForm.title,
            content: pushForm.content,
            priority: pushForm.priority
          }
          await pushMessageToAll(data)
        } else {
          const data: PushMessageToRoleRequest = {
            roleIds: pushForm.roleIds,
            title: pushForm.title,
            content: pushForm.content,
            priority: pushForm.priority
          }
          await pushMessageToRole(data)
        }
        showSuccessNotification({ title: '成功', message: '消息推送成功' })
        pushDialogVisible.value = false
        await loadMessageData()
        await loadUnreadCount()
      } catch (error) {
        console.error('推送消息失败:', error)
        showErrorNotification({ title: '错误', message: '推送消息失败' })
      }
    }
  })
}

// 推送已有消息
const handlePushExisting = async () => {
  if (!pushExistingFormRef.value || !currentPushMessage.value) return
  const messageId = currentPushMessage.value.messageId
  await pushExistingFormRef.value.validate(async (valid) => {
    if (valid) {
      try {
        const data: PushExistingMessageRequest = {
          pushType: pushExistingForm.pushType,
          roleIds: pushExistingForm.roleIds && pushExistingForm.roleIds.length > 0 ? pushExistingForm.roleIds : undefined,
          userIds: pushExistingForm.userIds && pushExistingForm.userIds.length > 0 ? pushExistingForm.userIds : undefined
        }
        await pushExistingMessage(messageId, data)
        showSuccessNotification({ title: '成功', message: '消息推送成功' })
        pushExistingDialogVisible.value = false
        await loadMessageData()
        await loadUnreadCount()
      } catch (error) {
        console.error('推送消息失败:', error)
        showErrorNotification({ title: '错误', message: '推送消息失败' })
      }
    }
  })
}

const viewDetail = async (row: UserMessageDto) => {
  try {
    // 获取用户消息详情
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
    await ElMessageBox.confirm('确认将所有未读消息标记为已读吗？', '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'info'
    })

    await markAllAsRead()
    showSuccessNotification({ title: '成功', message: '已全部标记为已读' })
    await loadUnreadCount()
    await loadMessageData()
  } catch (error) {
    console.log('取消操作')
  }
}

const handleDelete = async (row: UserMessageDto) => {
  try {
    await ElMessageBox.confirm('确认删除该消息吗？', '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })

    const response = await deleteUserMessage(row.recipientId)
    if (response.success === false) {
      showErrorNotification({ title: '错误', message: response.message || '删除失败' })
      return
    }
    showSuccessNotification({ title: '成功', message: '删除成功' })
    await loadUnreadCount()
    await loadMessageData()
  } catch (error) {
    console.log('取消删除或删除失败')
  }
}

// 撤回单条消息
const handleRevoke = async (row: UserMessageDto) => {
  try {
    await ElMessageBox.confirm('确认撤回该消息吗？撤回后消息将作废，但接收者仍可查看。', '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })

    const response = await revokeMessage(row.messageId)
    if (response.data?.success === false || response.success === false) {
      showErrorNotification({ title: '错误', message: response.message || '撤回消息失败' })
      return
    }
    showSuccessNotification({ title: '成功', message: '消息已撤回' })
    await loadUnreadCount()
    await loadMessageData()
  } catch (error) {
    if (error !== 'cancel') {
      console.error('撤回消息失败:', error)
      showErrorNotification({ title: '错误', message: '撤回消息失败' })
    }
  }
}

// 批量撤回消息
const handleBatchRevoke = async () => {
  if (selectedIds.value.length === 0) {
    showErrorNotification({ title: '提示', message: '请选择要撤回的消息' })
    return
  }

  try {
    await ElMessageBox.confirm(`确认撤回选中的 ${selectedIds.value.length} 条消息吗？撤回后消息将作废，但接收者仍可查看。`, '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })

    const messageIds = selectedMessages.value.map(msg => msg.messageId)
    const response = await batchRevokeMessages(messageIds)
    if (response.data?.success === false || response.success === false) {
      showErrorNotification({ title: '错误', message: response.message || '批量撤回失败' })
      return
    }
    showSuccessNotification({ title: '成功', message: '批量撤回成功' })
    selectedIds.value = []
    await loadUnreadCount()
    await loadMessageData()
  } catch (error) {
    if (error !== 'cancel') {
      console.error('批量撤回失败:', error)
      showErrorNotification({ title: '错误', message: '批量撤回失败' })
    }
  }
}

// 单个标记已读
const markSingleAsRead = async (row: UserMessageDto) => {
  // 已撤回的消息不能标记为已读
  if (row.isRevoked) {
    showErrorNotification({ title: '提示', message: '已撤回的消息不能标记为已读' })
    return
  }
  try {
    await markUserMessageAsRead(row.recipientId)
    showSuccessNotification({ title: '成功', message: '已标记为已读' })
    await loadUnreadCount()
    await loadMessageData()
  } catch (error) {
    console.error('操作失败:', error)
    showErrorNotification({ title: '错误', message: '操作失败' })
  }
}

const handleBatchDelete = async () => {
  // 过滤出可删除的消息（未读且没有其他用户已读）
  const deletableMessages = selectedMessages.value.filter(msg => !msg.isRead && !msg.hasBeenReadByOthers)

  if (deletableMessages.length === 0) {
    showErrorNotification({ title: '提示', message: '选中的消息中有其他用户已读，无法删除' })
    return
  }

  try {
    await ElMessageBox.confirm(`确认删除选中的 ${deletableMessages.length} 条消息吗？`, '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })

    const deletableIds = deletableMessages.map(msg => msg.recipientId)
    await batchDeleteUserMessages(deletableIds)
    showSuccessNotification({ title: '成功', message: '批量删除成功' })
    selectedIds.value = []
    await loadUnreadCount()
    await loadMessageData()
  } catch (error) {
    console.log('取消删除或删除失败')
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

onMounted(async () => {
  // 批量加载字典数据
  await loadDicts()
  // 加载用户和角色列表
  await loadUserList()
  await loadRoleList()
  // 加载消息数据
  await loadMessageData()
  await loadUnreadCount()
})
</script>

<style scoped>
.messages-container {
  padding: 0;
  height: 100%;
}

.messages-content {
  margin: 0;
  height: 100%;
}

.messages-card {
  height: 100%;
  border: none;
  border-radius: 0;
}

.card-header {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.header-left {
  display: flex;
  align-items: center;
}

.title-text {
  font-size: 16px;
  font-weight: bold;
}

.unread-badge {
  margin-right: 16px;
}

.filter-area {
  display: flex;
  gap: 12px;
  align-items: center;
  flex-wrap: wrap;
}

.filter-select {
  width: 120px;
}

.filter-input {
  width: 150px;
}

.table-actions {
  display: flex;
  gap: 8px;
  align-items: center;
}

:deep(.unread-row) {
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

.message-content-html {
  white-space: normal;
  word-break: break-word;
}

.message-content-html :deep(p) {
  margin: 0 0 8px;
}

.message-content-html :deep(p:last-child) {
  margin-bottom: 0;
}

.content-preview {
  max-height: 100px;
  overflow-y: auto;
  line-height: 1.6;
  white-space: pre-wrap;
  word-break: break-all;
}

.content-preview-html {
  white-space: normal;
}

.content-preview-html :deep(p) {
  margin: 0 0 6px;
}

.content-preview-html :deep(p:last-child) {
  margin-bottom: 0;
}

.revoked-message {
  text-decoration: line-through;
  color: #909399 !important;
  font-weight: normal !important;
  opacity: 0.6;
}

.dialog-footer {
  text-align: right;
}

.full-width {
  width: 100%;
}
</style>
