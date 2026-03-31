<template>
  <div class="clear-cache-page">
    <el-card class="page-card">
      <template #header>
        <div class="card-header">
          <span class="page-title">缓存管理</span>
          <el-tag type="info">共 {{ totalCount }} 项缓存</el-tag>
        </div>
      </template>

      <!-- 分类卡片区域 -->
      <div class="category-grid">
        <el-card
          v-for="category in categoryDetails"
          :key="category.key"
          class="category-card"
          :body-style="{ padding: '20px' }"
          shadow="hover"
        >
          <div class="category-content">
            <div class="category-icon" :style="{ color: category.color }">
              <el-icon :size="32">
                <component :is="getIconComponent(category.icon)" />
              </el-icon>
            </div>
            <div class="category-info">
              <h3 class="category-name">{{ category.name }}</h3>
              <p class="category-desc">{{ category.description }}</p>
              <el-tag :type="category.count > 0 ? 'success' : 'info'" size="small">
                {{ category.count }} 项
              </el-tag>
            </div>
            <el-button
              v-if="hasBtn(category.permission)"
              type="primary"
              :icon="Delete"
              circle
              :disabled="category.count === 0"
              @click="confirmClear(category.key)"
            />
          </div>
        </el-card>
      </div>

      <!-- 快捷操作区域 -->
      <div class="quick-actions">
        <el-divider>
          <el-icon><Operation /></el-icon>
          <span>快捷操作</span>
        </el-divider>
        <div class="action-buttons">
          <el-button
            v-if="hasBtn('cache:clear_auth')"
            type="warning"
            :icon="Key"
            @click="confirmClear('Auth')"
          >
            清除登录认证
          </el-button>
          <el-button
            v-if="hasBtn('cache:clear_user')"
            type="primary"
            :icon="User"
            @click="confirmClear('User')"
          >
            清除用户信息
          </el-button>
          <el-button
            v-if="hasBtn('cache:clear_menu')"
            type="success"
            :icon="Menu"
            @click="confirmClear('Menu')"
          >
            清除菜单缓存
          </el-button>
          <el-button
            v-if="hasBtn('cache:clear_list')"
            type="info"
            :icon="DataLine"
            @click="confirmClear('List')"
          >
            清除列表缓存
          </el-button>
          <el-button
            v-if="hasBtn('cache:clear_setting')"
            :icon="Setting"
            @click="confirmClear('Setting')"
          >
            清除设置缓存
          </el-button>
          <el-button
            v-if="hasBtn('cache:clear_all')"
            type="danger"
            :icon="Delete"
            @click="confirmClearAll"
          >
            清除全部缓存
          </el-button>
        </div>
      </div>

      <!-- 缓存统计表格 -->
      <div class="stats-section" v-if="showStats">
        <h3 class="section-title">
          <el-icon><DataAnalysis /></el-icon>
          缓存统计详情
        </h3>
        <el-table :data="statsTableData" border stripe style="width: 100%">
          <el-table-column prop="name" label="分类名称" width="150" />
          <el-table-column prop="count" label="缓存项数" width="100" align="center">
            <template #default="{ row }">
              <el-tag :type="row.count > 0 ? 'success' : 'info'" size="small">
                {{ row.count }}
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column prop="description" label="说明" />
          <el-table-column prop="keys" label="包含键名">
            <template #default="{ row }">
              <div class="key-tags">
                <el-tag
                  v-for="key in row.keyList"
                  :key="key"
                  size="small"
                  type="info"
                  class="key-tag"
                >
                  {{ key }}
                </el-tag>
              </div>
            </template>
          </el-table-column>
          <el-table-column label="操作" width="100" align="center">
            <template #default="{ row }">
              <el-button
                v-if="hasBtn(row.permission)"
                type="danger"
                :icon="Delete"
                size="small"
                :disabled="row.count === 0"
                @click="confirmClear(row.key)"
              >
                删除
              </el-button>
            </template>
          </el-table-column>
        </el-table>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessageBox } from 'element-plus'
import {
  Delete, Key, User, Menu, DataLine, Setting, Operation, DataAnalysis
} from '@element-plus/icons-vue'
import {
  clearByCategory,
  clearAllCache,
  getCategoryDetails,
  getTotalCacheCount,
  getKeysByCategory,
  type StorageCategory
} from '@/utils/storage'
import { showSuccessNotification } from '@/utils/notification'
import { useButtons } from '@/utils/buttons'

// 按钮管理
const { hasBtn } = useButtons('settings-system')

const router = useRouter()

// 状态
const showStats = ref(true)
const totalCount = ref(0)
const categoryDetails = ref<ReturnType<typeof getCategoryDetails>>([])

// 图标映射
const iconComponents: Record<string, any> = {
  Key,
  User,
  Menu,
  DataLine,
  Setting,
  Delete
}

const getIconComponent = (iconName: string) => {
  return iconComponents[iconName] || Setting
}

// 更新统计信息
const updateStats = () => {
  totalCount.value = getTotalCacheCount()
  categoryDetails.value = getCategoryDetails()
}

// 统计表格数据
const statsTableData = computed(() => {
  return categoryDetails.value.map(cat => ({
    key: cat.key,
    name: cat.name,
    count: cat.count,
    description: cat.description,
    keyList: getKeysByCategory(cat.key),
    permission: cat.permission
  }))
})

// 确认清除单个分类
const confirmClear = (category: StorageCategory) => {
  const categoryMap: Record<string, string> = {
    Auth: '登录认证',
    User: '用户信息',
    Menu: '菜单数据',
    List: '列表缓存',
    Setting: '系统设置'
  }

  const categoryName = categoryMap[category] || category

  ElMessageBox.confirm(
    `确定要清除「${categoryName}」缓存吗？${category === 'Auth' || category === 'User' ? '清除后需要重新登录。' : ''}`,
    '确认清除',
    {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    }
  ).then(() => {
    clearCategory(category)
  }).catch(() => {
    // 用户取消
  })
}

// 确认清除全部缓存
const confirmClearAll = () => {
  ElMessageBox.confirm(
    '确定要清除所有缓存吗？此操作将清除全部本地存储数据，清除后需要重新登录。',
    '确认清除全部',
    {
      confirmButtonText: '确定清除',
      cancelButtonText: '取消',
      type: 'warning',
      confirmButtonClass: 'el-button--danger'
    }
  ).then(() => {
    clearCategory('All')
  }).catch(() => {
    // 用户取消
  })
}

// 执行清除操作
const clearCategory = (category: StorageCategory) => {
  if (category === 'All') {
    clearAllCache()
    showSuccessNotification({ title: '成功', message: '所有缓存已清除' })
    setTimeout(() => {
      window.location.href = '/'
    }, 1500)
    return
  }

  clearByCategory(category)

  const messages: Record<string, string> = {
    Auth: '登录认证缓存已清除',
    User: '用户信息缓存已清除',
    Menu: '菜单缓存已清除',
    List: '列表缓存已清除',
    Setting: '系统设置缓存已清除'
  }

  showSuccessNotification({ title: '成功', message: messages[category] || '缓存已清除' })

  // 如果清除的是认证或用户信息，延迟跳转到登录页
  if (category === 'Auth' || category === 'User') {
    setTimeout(() => {
      router.push('/')
    }, 1500)
  } else {
    updateStats()
  }
}

// 组件挂载时更新统计
onMounted(() => {
  updateStats()
})
</script>

<style scoped>
.clear-cache-page {
  padding: 20px;
}

.page-card {
  border-radius: 8px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.page-title {
  font-size: 20px;
  font-weight: 600;
  color: #333;
}

/* 分类卡片网格 */
.category-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
  gap: 16px;
  margin-bottom: 20px;
}

.category-card {
  border-radius: 8px;
  transition: transform 0.2s, box-shadow 0.2s;
}

.category-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.category-content {
  display: flex;
  align-items: center;
  gap: 16px;
}

.category-icon {
  flex-shrink: 0;
  width: 56px;
  height: 56px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #f5f7fa;
  border-radius: 12px;
}

.category-info {
  flex: 1;
  min-width: 0;
}

.category-name {
  font-size: 16px;
  font-weight: 600;
  color: #303133;
  margin: 0 0 4px 0;
}

.category-desc {
  font-size: 12px;
  color: #909399;
  margin: 0 0 8px 0;
  line-height: 1.4;
}

/* 快捷操作区域 */
.quick-actions {
  margin-top: 20px;
}

.action-buttons {
  display: flex;
  gap: 12px;
  flex-wrap: wrap;
  margin-top: 16px;
}

/* 统计区域 */
.stats-section {
  margin-top: 24px;
}

.section-title {
  font-size: 16px;
  font-weight: 600;
  color: #333;
  margin-bottom: 16px;
  display: flex;
  align-items: center;
  gap: 8px;
}

.key-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 4px;
}

.key-tag {
  margin: 2px;
}

/* 响应式 */
@media (max-width: 768px) {
  .category-grid {
    grid-template-columns: 1fr;
  }

  .action-buttons {
    flex-direction: column;
  }

  .action-buttons .el-button {
    width: 100%;
  }
}
</style>