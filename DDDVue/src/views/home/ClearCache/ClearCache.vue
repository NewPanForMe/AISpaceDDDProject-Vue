<template>
  <div class="clear-cache-page">
    <h2 class="page-title">按分类清除</h2>
    <!-- 分类清除区域 -->
    <div class="category-section">
      <div class="category-buttons">
        <el-button type="primary" @click="clearCategory('Login')" :icon="User">
          清除登录缓存
        </el-button>
        <el-button type="success" @click="clearCategory('Menu')" :icon="Menu">
          清除菜单缓存
        </el-button>
        <el-button type="info" @click="clearCategory('List')" :icon="DataLine">
          清除列表缓存
        </el-button>
        <el-button type="warning" @click="clearCategory('All')" :icon="Setting">
          清除全部缓存
        </el-button>
      </div>
    </div>

    <!-- 缓存统计 -->
    <div class="stats-section" v-if="showStats">
      <h3 class="section-title">当前缓存统计</h3>
      <el-descriptions :column="4" border>
        <el-descriptions-item label="登录缓存">
          {{ stats['Login'] || 0 }} 项
        </el-descriptions-item>
        <el-descriptions-item label="菜单缓存">
          {{ stats['Menu'] || 0 }} 项
        </el-descriptions-item>
        <el-descriptions-item label="列表缓存">
          {{ stats['List'] || 0 }} 项
        </el-descriptions-item>
        <el-descriptions-item label="其他缓存">
          {{ stats['Other'] || 0 }} 项
        </el-descriptions-item>
      </el-descriptions>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { Setting, User, Menu, DataLine } from '@element-plus/icons-vue'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { clearByCategory, getStorageStats, StorageKeys } from '@/utils/storage'

const router = useRouter()
const showStats = ref(false)
const stats = ref<Record<string, number>>({})

// 更新统计信息
const updateStats = () => {
  stats.value = getStorageStats()
  showStats.value = true
}

// 清除所有缓存
const clearAllCache = () => {
  clearByCategory('All')
  ElMessage.success('所有缓存已清理')
  setTimeout(() => { window.location.href = '/' }, 1500)
}

// 退出登录
const handleLogout = () => {
  clearByCategory('Login')
  ElMessage.success('已退出登录')
  setTimeout(() => { router.push('/') }, 1500)
}

// 清理列表缓存
const clearMenuCache = () => {
  clearByCategory('Menu')
  ElMessage.success('列表缓存已清理')
  setTimeout(() => { window.location.reload() }, 1500)
}

// 按分类清除
const clearCategory = (category: 'Login' | 'Menu' | 'List' | 'All') => {
  clearByCategory(category)

  const messages = {
    Login: '登录缓存已清除',
    Menu: '菜单缓存已清除',
    List: '列表缓存已清除',
    All: '所有缓存已清除'
  }

  ElMessage.success(messages[category])

  // 如果清除的是登录缓存，延迟跳转
  if (category === 'Login') {
    setTimeout(() => { router.push('/') }, 1500)
  } else if (category === 'All') {
    setTimeout(() => { window.location.href = '/' }, 1500)
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
  margin-top: 40px;
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
