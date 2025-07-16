import cv2
import face_recognition
import mysql.connector
import numpy as np
from datetime import datetime
import time
import os
from picamera2 import Picamera2
from PIL import Image

# Database connection configuration
db_config = {
    #host':  ,  Replace with your laptop's IP address 
    #'user': ,  # Replace with your MySQL username
    #'password':  , # Replace with your MySQL password
    #'database':   # Replace with your database name  
    }

# Initialize Raspberry Pi camera
picam2 = Picamera2()
picam2.configure(picam2.create_preview_configuration(main={"size": (640, 480)}))
picam2.start()

# Initialize face cascade for detection
face_cascade = cv2.CascadeClassifier('/home/pi/Documents/haarcascade_frontalface_default.xml')

def connect_to_db():
    try:
        conn = mysql.connector.connect(**db_config)
        return conn
    except mysql.connector.Error as err:
        print(f"Error connecting to database: {err}")
        return None

def fetch_student_data():
    conn = connect_to_db()
    if not conn:
        return None
    
    try:
        cursor = conn.cursor(dictionary=True)
        query = "SELECT Matric_Number, ImageData, Course_code FROM Details"
        cursor.execute(query)
        Details = cursor.fetchall()
        cursor.close()
        conn.close()
        return Details
    except mysql.connector.Error as err:
        print(f"Error fetching student data: {err}")
        return None

def extract_face_encodings(image_blob):
    # Convert BLOB to image
    nparr = np.frombuffer(image_blob, np.uint8)
    img = cv2.imdecode(nparr, cv2.IMREAD_COLOR)
    
    # Convert to RGB for face_recognition
    rgb_img = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)
    
    # Detect faces
    encodings = face_recognition.face_encodings(rgb_img)
    return encodings[0] if encodings else None

def log_attendance(Matric_Number, status):
    conn = connect_to_db()
    if not conn:
        return False
    
    try:
        cursor = conn.cursor()
        query = """
        INSERT INTO ECE5251 (Matric_Number, status, timestamp)
        VALUES (%s, %s, %s)
        """
        timestamp = datetime.now()
        cursor.execute(query, (Matric_Number, status, timestamp))
        conn.commit()
        cursor.close()
        conn.close()
        return True
    except mysql.connector.Error as err:
        print(f"Error logging attendance: {err}")
        return False

def determine_status():
    current_time = datetime.now().time()
    # Example: Late after 9:00 AM
    late_threshold = datetime.strptime("09:00:00", "%H:%M:%S").time()
    return "late" if current_time > late_threshold else "present"

def main():
    # Fetch student data
    Details = fetch_student_data()
    if not Details:
        print("Failed to fetch student data")
        return
    
    # Store known face encodings
    known_encodings = []
    known_matric_numbers = []
    known_courses = []
    
    for Detail in Details:
        encoding = extract_face_encodings(Detail['ImageData'])
        if encoding is not None:
            known_encodings.append(encoding)
            known_matric_numbers.append(Detail['Matric_Number'])
            known_courses.append(Detail['Course_code'].split(','))  # Assuming courses are comma-separated
    
    print("Starting attendance system...")
    
    while True:
        # Capture frame from camera
        frame = picam2.capture_array()
        rgb_frame = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
        
        # Detect faces in frame
        faces = face_cascade.detectMultiScale(cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY), 1.3, 5)
        
        for (x, y, w, h) in faces:
            # Ensure face_roi is valid
            if w > 0 and h > 0:  # Check for non-empty region
                # Extract face region
                face_roi = rgb_frame[y:y+h, x:x+w]
                if face_roi.size == 0 or len(face_roi.shape) != 3 or face_roi.shape[2] != 3:
                    print(f"Invalid face_roi shape: {face_roi.shape}")
                    continue
            
                # Get face encoding
                encodings = face_recognition.face_encodings(face_roi)
                if not encodings:
                    print("No encodings found for face")
                    continue
                    
                encoding = encodings[0]
                
                # Compare with known faces
                matches = face_recognition.compare_faces(known_encodings, encoding)
                face_distances = face_recognition.face_distance(known_encodings, encoding)
                
                if len(face_distances) > 0:
                    best_match_index = np.argmin(face_distances)
                    
                    if matches[best_match_index]:
                        matric_number = known_matric_numbers[best_match_index]
                        courses = known_courses[best_match_index]
                        status = determine_status()
                        
                        # Log attendance for each course
                        for course in courses:
                            if log_attendance(matric_number, course.strip(), status):
                                print(f"Attendance logged for {matric_number} in {course} as {status}")
                            else:
                                print(f"Failed to log attendance for {matric_number} in {course}")
                
                # Draw rectangle around face
                cv2.rectangle(frame, (x, y), (x+w, y+h), (0, 255, 0), 2)
        
        # Display the frame
        cv2.imshow('Attendance System', frame)
        
        # Break loop on 'q' press
        if cv2.waitKey(1) & 0xFF == ord('q'):
            break
    
    # Cleanup
    picam2.stop()
    cv2.destroyAllWindows()

if __name__ == "__main__":
    main()
