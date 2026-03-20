<template>
  <div class="settings-page">
    <h2 class="page-title">系统设置</h2>
    <div class="settings-content">
      <el-card>
        <p class="placeholder-text">系统设置功能开发中...</p>
      </el-card>
    </div>
  </div>
</template>

<script setup lang="ts">
</script>

<style scoped>
.settings-page {
  padding: 20px;
}

.page-title {
  font-size: 24px;
  font-weight: 600;
  color: #333;
  margin-bottom: 20px;
}

.settings-content {
  height: calc(100vh - 150px);
}

.placeholder-text {
  text-align: center;
  color: #666;
  font-size: 16px;
  padding: 40px;
}
</style>
