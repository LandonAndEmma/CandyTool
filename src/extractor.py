import os

def extract_files(input_file):
    with open(input_file, 'rb') as f:
        data = f.read()

    index = 0
    while index < len(data):
        if data[index:index+4] == b'SWAR':
            file_type = ".swar"
            index += 8  # Move to the size field
            file_size = int.from_bytes(data[index:index+4], byteorder='little')
            index += 4  # Move past the size field
            header_data = b'SWAR' + data[index-8:index]  # Prepend header with "SWAR"
        elif data[index:index+4] == b'SBNK':
            file_type = ".sbnk"
            index += 8  # Move to the size field
            file_size = int.from_bytes(data[index:index+4], byteorder='little')
            index += 4  # Move past the size field
            header_data = b'SBNK' + data[index-8:index]  # Prepend header with "SBNK"
        else:
            index += 1
            continue

        # Create the 'extracted' folder if it doesn't exist
        if not os.path.exists("extracted"):
            os.mkdir("extracted")

        # Extract the header and the content
        output_filename = f"extracted/extracted_{index}_{file_type}"
        with open(output_filename, 'wb') as output_file:
            output_file.write(header_data)  # Write the header
            output_file.write(data[index:index+file_size])  # Write the content

        # Move index to the end of the extracted file or until "DD" is encountered
        while index < len(data) and data[index] != 0xDD:
            index += 1

        if index < len(data):
            index += 1  # Move past the "DD" marker

if __name__ == "__main__":
    input_file = input("Enter the path to the file containing the embedded SWAR and SBNK files: ")
    if os.path.exists(input_file):
        extract_files(input_file)
        print("Extraction completed successfully.")
    else:
        print("File not found.")
