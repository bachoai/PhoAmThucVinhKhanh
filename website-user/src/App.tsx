import { PresenceHeartbeat } from './app/PresenceHeartbeat';
import { SessionBootstrap } from './app/SessionBootstrap';
import { AppRoutes } from './routes/AppRoutes';

export default function App() {
  return (
    <>
      <SessionBootstrap />
      <PresenceHeartbeat />
      <AppRoutes />
    </>
  );
}
