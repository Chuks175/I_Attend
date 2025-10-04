import datetime
import os
import cv2
import cv2 as cv
import face_recognition
from datetime import datetime
import time
import mysql.connector
import numpy as np


db_config = {
    'host': 'db26011.public.databaseasp.net',
    'user': 'db26011',
    'password': '5d=N-Xc9e3L#',
    'database': 'db26011'
}

def display(message):
    print(f"{message}")
    return message

cap = cv2.VideoCapture(0)
if cap is None:
    display("Trying USB Camera...")
    cap = cv2.VideoCapture(1)
if cap is None:
    display("No Camera Found ")
    print("No Camera Available")
    time.sleep(10)
    exit()

def connect_to_db():
    try:
        conn = mysql.connector.connect(**db_config)
        return conn
    except mysql.connector.Error as err:
        display(f"Error Connecting to database: {err}")
        print(f"Error Connecting to database: {err}")
        return None
    
def fetch_student_data():
    conn = connect_to_db()
    if not conn:
        return None
    try:
        cursor = conn.cursor(dictionary=True)
        query = "SELECT Matric_Number, ImageData, Course_code FROM Details"
        cursor.execute(query)
        students = cursor.fetchall()
        cursor.close()
        conn.close()
        return students
    except mysql.connector.Error as err:
        display(f"Error Fetching Students Data: {err}")
        return None
    
def extract_face_encodings(image_blob):
    try:
        nparr = np.frombuffer(image_blob, np.uint8)
        img = cv2.imdecode(nparr, cv2.IMREAD_COLOR)
        if img is None:
            display("Failed to Decode Image From Database")
            print("Failed to decode image from database!")
            return None
        rgb_img = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)
        encodings = face_recognition.face_encodings(rgb_img, model="hog")
        return encodings[0] if encodings else None
    except Exception as e:
        display(f"Error in Extracting Facial Encodings:{e}")
        print(f"Error in extract_face_encodings: {e}")
        return None
    
def log_attendance(Matric_Number, status):
    conn = connect_to_db()
    if not conn:
        display(f"Failed: {Matric_Number[:8]}")
        time.sleep(5)
        return False
    try:
        cursor = conn.cursor()
        query = """
        INSERT INTO ECE5251 (Matric_Number, date, time, status)
        VALUES (%s, %s, %s, %s)
        """
        current_date = datetime.now().date()
        current_time = datetime.now().time()
        cursor.execute(query, (Matric_Number, current_date, current_time, status))
        conn.commit()
        cursor.close()
        conn.close()
        display(f"OK: {Matric_Number[:8]}")
        time.sleep(5)
        return True
    except mysql.connector.Error as err:
        print(f"Error in Logging attendance: {err}")
        display(f"Failed: {Matric_Number[:8]}")
        time.sleep(5)
        return False
    
def determine_status():
    current_time = datetime.now().time()
    late_threshold = datetime.strptime("09:00:00", "%H:%M:%S").time()
    return "late" if current_time >= late_threshold else "present"

def main():
    students = fetch_student_data()
    if not students:
        display("DB Fetch Failed")
        print("Failed to fetch students data")
        time.sleep(5)
        return
    
    known_encodings=[]
    known_matric_numbers=[]
    for student in students:
        encoding = extract_face_encodings(student["ImageData"])
        if encoding is not None:
            known_encodings.append(encoding)
            known_matric_numbers.append(student["Matric_Number"])
            
    print(f"Loaded {len(known_encodings)} student encodings")
    display(f"Loaded {len(known_encodings)} encodings")
    time.sleep(2)
    
    frame_count = 0
    while True:
        try:
            ret, frame = cap.read()
            if not ret or frame is None or frame.size == 0:
                print("Failed to capture frame")
                display("Frame Capture Fail")
                time.sleep(2)
                continue
            rgb_frame = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
            if frame_count % 10 == 0:
                cv.imwrite(f"captured_frame_{time.time()}.jpg", cv2.cvtColor(rgb_frame, cv2.COLOR_RGB2BGR))
            frame_count += 1


            rgb_frame = cv2.convertScaleAbs(rgb_frame, alpha=1.3, beta=50)
            face_locations = face_recognition.face_locations(rgb_frame, number_of_times_to_upsample=2, model="hog")
            print(f"Detected {len(face_locations)} faces")

            if len(face_locations) == 0:
                display("No Face Detected")
                time.sleep(1)


            for (top, right, bottom, left) in face_locations:
                w, h = right - left, bottom - top
                print(f"Face at top={top}, right={right}, bottom={bottom}, left={left}, w={w}, h={h}")
                if w < 50 or h < 50:
                    print(f"Face too small: w={w}, h={h}")
                    display('Face Too Small')
                    time.sleep(2)
                    continue

                face_roi = rgb_frame[top:bottom, left:right]
                if face_roi.size == 0 or len(face_roi.shape) != 3 or face_roi.shape[2] != 3:
                    print(f"Invalid face_roi shape: {face_roi.shape}")
                    display('Invalid Face ROI')
                    time.sleep(2)
                    continue

                print(f"face_roi shape: {face_roi.shape}, dtype: {face_roi.dtype}")
                face_roi = cv2.convertScaleAbs(face_roi, alpha=1.3, beta=50)  # Enhance contrast
                face_roi = cv2.resize(face_roi, (128, 128))  # Resize for consistency

                encodings = face_recognition.face_encodings(face_roi, model="hog")
                if not encodings:
                    print("No encodings found for face")
                    display('No Encoding Found')
                    time.sleep(2)
                    continue

                encoding = encodings[0]

                matches = face_recognition.compare_faces(known_encodings, encoding, tolerance=0.6)
                face_distances = face_recognition.face_distance(known_encodings, encoding)

                if len(face_distances) > 0:
                    best_match_index = np.argmin(face_distances)
                    if matches[best_match_index]:
                        matric_number = known_matric_numbers[best_match_index]
                        status = determine_status()
                        if log_attendance(matric_number, status):
                            print(f"Attendance logged for {matric_number} as {status}")
                        else:
                            print(f"Failed to log attendance for {matric_number}")

                cv2.rectangle(frame, (left, top), (right, bottom), (0, 255, 0), 2)


            cv2.imshow('Attendance System', frame)

                # cv2.rectangle(frame, (x, y), (y+h, x+w), (0, 255, 0), 2)
                # cv2.imshow('Attendance System', frame)

            if cv2.waitKey(1) & 0xFF == ord('q'):
                break


        except Exception as e:
            print(f"Error in main loop: {e}")
            display("Error in Main Loop")
            time.sleep(5)
            continue

    cap.release()
    cv2.destroyAllWindows()
    display('System Stopped')


if __name__ == "__main__":
    main()






