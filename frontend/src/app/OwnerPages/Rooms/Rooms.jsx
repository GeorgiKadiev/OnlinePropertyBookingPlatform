import React, { useState, useEffect } from "react";
import { useParams } from "react-router-dom";
import {
  Box,
  Card,
  CardContent,
  Typography,
  Button,
  TextField,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  CircularProgress,
} from "@mui/material";
import NavBar from "../../../components/NavBar/NavBar";
import { useSelector } from "react-redux";

export default function Rooms() {
  const { id } = useParams(); // Get the estateId from the URL
  const token = useSelector((state) => state.token); // Retrieve the token from Redux store

  const [rooms, setRooms] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [openDialog, setOpenDialog] = useState(false);
  const [selectedRoomId, setSelectedRoomId] = useState(null);
  const [photoLink, setPhotoLink] = useState("");

  // Fetch rooms for the estate
  useEffect(() => {
    const fetchRooms = async () => {
      try {
        const response = await fetch(
          `http://localhost:5076/api/estate/${id}/rooms`,
          {
            method: "GET",
            headers: {
              "Content-Type": "application/json",
              Authorization: `Bearer ${token}`, // Pass token for authentication
            },
          }
        );

        if (!response.ok) {
          throw new Error("Failed to fetch rooms.");
        }

        const data = await response.json();
        setRooms(data);
        setLoading(false);
      } catch (err) {
        setError(err.message);
        setLoading(false);
      }
    };

    fetchRooms();
  }, [id, token]);

  const handleOpenDialog = (roomId) => {
    setSelectedRoomId(roomId); // Store the selected room ID
    setOpenDialog(true);
  };

  const handleCloseDialog = () => {
    setSelectedRoomId(null);
    setPhotoLink("");
    setOpenDialog(false);
  };

  const handleAddPhoto = async () => {
    if (!photoLink) {
      alert("Please provide a valid photo link.");
      return;
    }

    const payload = JSON.stringify({ url: photoLink });
    console.log(payload);

    try {
      const response = await fetch(
        `http://localhost:5076/api/room/${selectedRoomId}/add-photo`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`, // Include token for authentication
          },
          body: payload, // Pass the photo URL in the request body
        }
      );

      if (!response.ok) {
        throw new Error("Failed to add photo.");
      }

      alert("Photo added successfully!");
      handleCloseDialog();
    } catch (error) {
      console.error("Error adding photo:", error);
      alert("Failed to add photo. Please try again.");
    }
  };

  if (loading) {
    return (
      <Box
        sx={{
          display: "flex",
          justifyContent: "center",
          alignItems: "center",
          height: "100vh",
        }}
      >
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return (
      <Box
        sx={{
          display: "flex",
          justifyContent: "center",
          alignItems: "center",
          height: "100vh",
        }}
      >
        <Typography color="error">{error}</Typography>
      </Box>
    );
  }

  return (
    <div>
      <NavBar />
      <Box sx={{ padding: 4 }}>
        <Typography variant="h4" gutterBottom>
          Rooms of Estate {id}
        </Typography>

        {rooms.length === 0 ? (
          <Typography>No rooms available for this estate.</Typography>
        ) : (
          rooms.map((room) => (
            <Card
              key={room.id}
              sx={{ marginBottom: 2, padding: 2, display: "flex" }}
            >
              <Box
                sx={{
                  width: "20%",
                  display: "flex",
                  justifyContent: "center",
                  alignItems: "center",
                }}
              >
                <img
                  src={room.imageUrl || "https://via.placeholder.com/150"}
                  alt={room.name}
                  style={{ width: "100%", height: "auto", objectFit: "cover" }}
                />
              </Box>
              <CardContent sx={{ width: "60%" }}>
                <Typography variant="h5">{room.name}</Typography>
                <Typography variant="body2" color="textSecondary">
                  {room.description || "No description available"}
                </Typography>
              </CardContent>
              <Box sx={{ display: "flex", alignItems: "center" }}>
                <Button
                  variant="contained"
                  color="primary"
                  onClick={() => handleOpenDialog(room.id)} // Open dialog with room ID
                >
                  Add Photo
                </Button>
              </Box>
            </Card>
          ))
        )}
      </Box>

      {/* Dialog for adding photo */}
      <Dialog open={openDialog} onClose={handleCloseDialog}>
        <DialogTitle>Add Photo to Room</DialogTitle>
        <DialogContent>
          <TextField
            autoFocus
            margin="dense"
            label="Photo URL"
            type="url"
            fullWidth
            value={photoLink}
            onChange={(e) => setPhotoLink(e.target.value)}
            placeholder="Enter the photo URL"
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseDialog} color="secondary">
            Cancel
          </Button>
          <Button onClick={handleAddPhoto} color="primary">
            Add Photo
          </Button>
        </DialogActions>
      </Dialog>
    </div>
  );
}
