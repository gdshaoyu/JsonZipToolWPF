# JsonZipToolWPF

一个用于JSON压缩和解压的WPF工具。

## 功能特点

- JSON压缩：移除空格和换行符，减小文件体积
- JSON解压：格式化JSON，使其易于阅读
- JSON格式化：美化JSON显示格式
- 历史记录：保存最近的操作记录
- Material Design界面：现代化的用户界面设计

## 使用说明

1. 在输入框中粘贴或输入JSON文本
2. 点击相应按钮执行操作：
   - 压缩：移除所有空格和换行
   - 解压：格式化JSON使其易读
   - 格式化：美化JSON显示格式
3. 结果会自动显示在输出框中
4. 点击复制按钮可复制结果到剪贴板

## 技术栈

- .NET 9.0
- WPF (Windows Presentation Foundation)
- Material Design In XAML
- SQLite数据库
- Newtonsoft.Json 