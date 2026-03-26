<template>
  <div class="clear-cache-page">
    <el-card class="page-card">
      <h2 class="page-title">按分类清除</h2>
      <!-- 分类清除区域 -->
      <div class="category-section">
        <div class="category-buttons">
          <el-button v-if="hasPermission(PermissionCodes.CACHE_CLEAR_LOGIN)" type="primary" @click="clearCategory('Login')" :icon="User">
            清除登录缓存
          </el-button>
          <el-button v-if="hasPermission(PermissionCodes.CACHE_CLEAR_MENU)" type="success" @click="clearCategory('Menu')" :icon="Menu">
            清除菜单缓存
          </el-button>
          <el-button v-if="hasPermission(PermissionCodes.CACHE_CLEAR_LIST)" type="info" @click="clearCategory('List')" :icon="DataLine">
            清除列表缓存
          </el-button>
          <el-button v-if="hasPermission(PermissionCodes.CACHE_CLEAR_ALL)" type="warning" @click="clearCategory('All')" :icon="Setting">
            清除全部缓存
          </el-button>
        </div>
      </div>

      <!-- 缓存统计 -->
      <div class="stats-section" v-if="showStats">
        <h3 class="section-title">当前缓存统计</h3>
        <table class="stats-table">
          <tr>
            <td class="label">登录缓存</td>
            <td class="value">{{ stats['Login'] || 0 }} 项</td>
            <td class="label">菜单缓存</td>
            <td class="value">{{ stats['Menu'] || 0 }} 项</td>
          </tr>
          <tr>
            <td class="label">列表缓存</td>
            <td class="value">{{ stats['List'] || 0 }} 项</td>
            <td class="label">其他缓存</td>
            <td class="value">{{ stats['Other'] || 0 }} 项</td>
          </tr>
        </table>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { Setting, User, Menu, DataLine } from '@element-plus/icons-vue'
import { useRouter } from 'vue-router'
import { clearByCategory, getStorageStats, clearAllCache, hasPermission, PermissionCodes } from '@/utils/storage'
import { showSuccessNotification } from '@/utils/notification'

const router = useRouter()
const showStats = ref(false)
const stats = ref<Record<string, number>>({})

// 更新统计信息
const updateStats = () => {
  stats.value = getStorageStats()
  showStats.value = true
}

// 按分类清除
const clearCategory = (category: 'Login' | 'Menu' | 'List' | 'All') => {
  if (category === 'All') {
    clearAllCache()
    showSuccessNotification({ title: '成功', message: '所有缓存已清除' })
    setTimeout(() => { window.location.href = '/' }, 1500)
    return
  }

  clearByCategory(category)

  const messages = {
    Login: '登录缓存已清除',
    Menu: '菜单缓存已清除',
    List: '列表缓存已清除'
  }

  showSuccessNotification({ title: '成功', message: messages[category] })

  // 如果清除的是登录缓存，延迟跳转
  if (category === 'Login') {
    setTimeout(() => { router.push('/') }, 1500)
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

.page-title {
  font-size: 24px;
  font-weight: 600;
  color: #333;
  margin-bottom: 20px;
}

.cache-buttons {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 20px;
  max-width: 1200px;
}

.cache-card {
  transition: transform 0.3s, box-shadow 0.3s;
  cursor: pointer;
}

.cache-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.card-content {
  text-align: center;
  padding: 30px 20px;
}

.card-icon {
  color: #4361ee;
  margin-bottom: 16px;
}

.card-title {
  font-size: 18px;
  font-weight: 600;
  color: #333;
  margin: 0 0 8px 0;
}

.card-desc {
  font-size: 14px;
  color: #666;
  margin: 0 0 20px 0;
  line-height: 1.6;
}

.action-btn {
  width: 100%;
}

/* 分类清除区域 */
.category-section {
  margin-top: 20px;
  padding: 20px;
  background: #f5f7fa;
  border-radius: 8px;
}

.section-title {
  font-size: 18px;
  font-weight: 600;
  color: #333;
  margin-bottom: 16px;
}

.category-buttons {
  display: flex;
  gap: 12px;
  flex-wrap: wrap;
}

/* 统计区域 */
.stats-section {
  margin-top: 20px;
  padding: 20px;
  background: #f5f7fa;
  border-radius: 8px;
}

/* 原生表格样式 */
.stats-table {
  width: 100%;
  border-collapse: collapse;
  border: 1px solid #ebeef5;
  background: #fff;
}

.stats-table tr {
  border-bottom: 1px solid #ebeef5;
}

.stats-table tr:last-child {
  border-bottom: none;
}

.stats-table td {
  padding: 12px 16px;
  border-right: 1px solid #ebeef5;
}

.stats-table td:last-child {
  border-right: none;
}

.stats-table .label {
  background: #fafafa;
  font-weight: 500;
  color: #606266;
  width: 25%;
}

.stats-table .value {
  color: #303133;
  width: 25%;
}

@media (max-width: 768px) {
  .cache-buttons {
    grid-template-columns: 1fr;
  }

  .category-buttons {
    flex-direction: column;
  }

  .category-buttons .el-button {
    width: 100%;
  }
}
</style>
