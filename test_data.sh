#!/bin/bash

# API Base URL
BASE_URL="http://localhost:5232/api"

echo "🚀 Test verileri oluşturuluyor..."
echo ""

# 1. Instructors oluştur
echo "📚 1. Eğitmenler oluşturuluyor..."
curl -s -X POST "$BASE_URL/Instructors" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Ahmet",
    "surname": "Yılmaz",
    "email": "ahmet.yilmaz@example.com",
    "professions": "Yazılım Geliştirme",
    "phoneNumber": "05551234567"
  }' | jq .

echo ""
curl -s -X POST "$BASE_URL/Instructors" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Ayşe",
    "surname": "Demir",
    "email": "ayse.demir@example.com",
    "professions": "Veri Bilimi",
    "phoneNumber": "05559876543"
  }' | jq .

echo ""
sleep 1

# 2. Tüm Instructors'ı getir ve ID'leri al
echo "📋 2. Eğitmenler listeleniyor..."
INSTRUCTORS=$(curl -s -X GET "$BASE_URL/Instructors")
INSTRUCTOR_ID1=$(echo $INSTRUCTORS | jq -r '.data[0].id // empty')
INSTRUCTOR_ID2=$(echo $INSTRUCTORS | jq -r '.data[1].id // empty')

echo "Eğitmen ID'leri: $INSTRUCTOR_ID1, $INSTRUCTOR_ID2"
echo ""

# 3. Students oluştur
echo "👨‍🎓 3. Öğrenciler oluşturuluyor..."
curl -s -X POST "$BASE_URL/Students" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Mehmet",
    "surname": "Kaya",
    "birthDate": "2000-01-15T00:00:00",
    "tc": "12345678901"
  }' | jq .

echo ""
curl -s -X POST "$BASE_URL/Students" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Zeynep",
    "surname": "Çelik",
    "birthDate": "1999-05-20T00:00:00",
    "tc": "98765432109"
  }' | jq .

echo ""
sleep 1

# 4. Tüm Students'ı getir ve ID'leri al
echo "📋 4. Öğrenciler listeleniyor..."
STUDENTS=$(curl -s -X GET "$BASE_URL/Students")
STUDENT_ID1=$(echo $STUDENTS | jq -r '.data[0].id // empty')
STUDENT_ID2=$(echo $STUDENTS | jq -r '.data[1].id // empty')

echo "Öğrenci ID'leri: $STUDENT_ID1, $STUDENT_ID2"
echo ""

# 5. Courses oluştur
echo "📖 5. Kurslar oluşturuluyor..."
curl -s -X POST "$BASE_URL/Courses" \
  -H "Content-Type: application/json" \
  -d "{
    \"courseName\": \"C# Programlama Temelleri\",
    \"startDate\": \"2024-01-01T00:00:00\",
    \"endDate\": \"2024-06-30T00:00:00\",
    \"instructorID\": \"$INSTRUCTOR_ID1\",
    \"isActive\": true
  }" | jq .

echo ""
curl -s -X POST "$BASE_URL/Courses" \
  -H "Content-Type: application/json" \
  -d "{
    \"courseName\": \"Python ile Veri Analizi\",
    \"startDate\": \"2024-02-01T00:00:00\",
    \"endDate\": \"2024-07-31T00:00:00\",
    \"instructorID\": \"$INSTRUCTOR_ID2\",
    \"isActive\": true
  }" | jq .

echo ""
sleep 1

# 6. Tüm Courses'ı getir ve ID'leri al
echo "📋 6. Kurslar listeleniyor..."
COURSES=$(curl -s -X GET "$BASE_URL/Courses")
COURSE_ID1=$(echo $COURSES | jq -r '.data[0].id // empty')
COURSE_ID2=$(echo $COURSES | jq -r '.data[1].id // empty')

echo "Kurs ID'leri: $COURSE_ID1, $COURSE_ID2"
echo ""

# 7. Lessons oluştur
echo "📚 7. Dersler oluşturuluyor..."
curl -s -X POST "$BASE_URL/Lessons" \
  -H "Content-Type: application/json" \
  -d "{
    \"title\": \"C# Temel Syntax\",
    \"date\": \"2024-01-15T10:00:00\",
    \"duration\": 90,
    \"content\": \"C# programlama temelleri\",
    \"courseID\": \"$COURSE_ID1\",
    \"time\": \"10:00\"
  }" | jq .

echo ""
curl -s -X POST "$BASE_URL/Lessons" \
  -H "Content-Type: application/json" \
  -d "{
    \"title\": \"Python Pandas Kütüphanesi\",
    \"date\": \"2024-02-15T14:00:00\",
    \"duration\": 120,
    \"content\": \"Pandas ile veri analizi\",
    \"courseID\": \"$COURSE_ID2\",
    \"time\": \"14:00\"
  }" | jq .

echo ""
sleep 1

# 8. Exams oluştur
echo "📝 8. Sınavlar oluşturuluyor..."
curl -s -X POST "$BASE_URL/Exams" \
  -H "Content-Type: application/json" \
  -d "{
    \"name\": \"C# Vize Sınavı\",
    \"date\": \"2024-03-15T10:00:00\"
  }" | jq .

echo ""
curl -s -X POST "$BASE_URL/Exams" \
  -H "Content-Type: application/json" \
  -d "{
    \"name\": \"Python Final Sınavı\",
    \"date\": \"2024-08-01T14:00:00\"
  }" | jq .

echo ""
sleep 1

# 9. Tüm Exams'ı getir ve ID'leri al
echo "📋 9. Sınavlar listeleniyor..."
EXAMS=$(curl -s -X GET "$BASE_URL/Exams")
EXAM_ID1=$(echo $EXAMS | jq -r '.data[0].id // empty')
EXAM_ID2=$(echo $EXAMS | jq -r '.data[1].id // empty')

echo "Sınav ID'leri: $EXAM_ID1, $EXAM_ID2"
echo ""

# 10. Registrations oluştur
echo "📝 10. Kayıtlar oluşturuluyor..."
curl -s -X POST "$BASE_URL/Registrations" \
  -H "Content-Type: application/json" \
  -d "{
    \"studentID\": \"$STUDENT_ID1\",
    \"courseID\": \"$COURSE_ID1\",
    \"price\": 5000.00,
    \"registrationDate\": \"2024-01-01T00:00:00\"
  }" | jq .

echo ""
curl -s -X POST "$BASE_URL/Registrations" \
  -H "Content-Type: application/json" \
  -d "{
    \"studentID\": \"$STUDENT_ID2\",
    \"courseID\": \"$COURSE_ID2\",
    \"price\": 6000.00,
    \"registrationDate\": \"2024-02-01T00:00:00\"
  }" | jq .

echo ""
sleep 1

# 11. ExamResults oluştur
echo "📊 11. Sınav Sonuçları oluşturuluyor..."
curl -s -X POST "$BASE_URL/ExamResults" \
  -H "Content-Type: application/json" \
  -d "{
    \"studentID\": \"$STUDENT_ID1\",
    \"examID\": \"$EXAM_ID1\",
    \"grade\": 85
  }" | jq .

echo ""
curl -s -X POST "$BASE_URL/ExamResults" \
  -H "Content-Type: application/json" \
  -d "{
    \"studentID\": \"$STUDENT_ID2\",
    \"examID\": \"$EXAM_ID2\",
    \"grade\": 92
  }" | jq .

echo ""
echo "✅ Tüm test verileri başarıyla oluşturuldu!"
echo ""
echo "📋 Özet:"
echo "   - 2 Eğitmen"
echo "   - 2 Öğrenci"
echo "   - 2 Kurs"
echo "   - 2 Ders"
echo "   - 2 Sınav"
echo "   - 2 Kayıt"
echo "   - 2 Sınav Sonucu"
