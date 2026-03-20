<template>
  <div class="dashboard-page">
    <h2 class="page-title">首页</h2>
    <div class="dashboard-content">
      <el-card class="card">
        <div class="card-content">
          <h3>欢迎使用智能管理系统</h3>
          <p>这是一个基于 Vue 3 + TypeScript + Element Plus 构建的现代化管理系统。</p>
        </div>
      </el-card>
    </div>
  </div>
</template>

<script setup lang="ts">
</script>

<style scoped>
.dashboard-page {
  padding: 20px;
}

.page-title {
  font-size: 24px;
  font-weight: 600;
  color: #333;
  margin-bottom: 20px;
}

.dashboard-content {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 20px;
}

.card {
  transition: transform 0.3s;
}

.card:hover {
  transform: translateY(-4px);
  transform: translateX(-4px);
}

.card-content {
  text-align: center;
  padding: 20px;
}

.card-content h3 {
  font-size: 18px;
  font-weight: 600;
  color: #333;
  margin-bottom: 10px;
}

.card-content p {
  font-size: 14px;
  color: #666;
  margin: 0;
}
</style>
